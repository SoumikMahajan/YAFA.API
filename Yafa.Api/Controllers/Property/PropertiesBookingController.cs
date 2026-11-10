using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.V2.Core;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Payments;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Controllers.Property
{
	/// <summary>
	/// Provides API endpoints for managing property bookings, including retrieving availability, creating and modifying
	/// bookings, handling payments, and submitting property ratings and reviews.
	/// </summary>
	/// <remarks>All endpoints require user authorization. This controller coordinates booking-related
	/// operations such as checking property availability, creating hold bookings, initiating and confirming payments,
	/// retrieving user bookings, canceling or extending bookings, and submitting property reviews. Responses are
	/// standardized using the API response service. Ensure that all required identifiers and request data are valid
	/// when calling these endpoints.</remarks>
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PropertiesBookingController : ControllerBase
	{
		private readonly ILogService _logService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApiResponseService _responseService;
		private readonly IFileRepo _fileService;
		private readonly IFirebaseService _firebase;
		private readonly StripeSettings _stripeSettings;
		private readonly decimal _partialPaymentPercentage;

		/// <summary>
		/// Initializes a new instance of the PropertiesBookingController class with the specified services and
		/// configuration.
		/// </summary>
		/// <param name="logService">The logging service used to record application events and errors.</param>
		/// <param name="unitOfWork">The unit of work instance that manages database transactions and repository access.</param>
		/// <param name="configuration">The application configuration settings used for controller operations.</param>
		/// <param name="responseService">The service used to generate standardized API responses.</param>
		/// <param name="fileService">The file repository service used for file management operations.</param>
		/// <param name="firebase">The Firebase service used for interacting with Firebase-related features.</param>
		/// <param name="stripeSettings">The Stripe settings for payment processing.</param>
		public PropertiesBookingController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService, IFirebaseService firebase, IOptions<StripeSettings> stripeSettings)
		{
			_logService = logService;
			_unitOfWork = unitOfWork;
			_responseService = responseService;
			_fileService = fileService;
			_firebase = firebase;
			_stripeSettings = stripeSettings.Value;
			_partialPaymentPercentage = configuration.GetValue<decimal>("PaymentSettings:PartialPaymentPercentage", 30m);

			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
		}


		#region :: Booking

		/// <summary>
		/// Creates a hold booking for a property, reserving the specified dates and guest count while calculating the total amount and partial payment amounts.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("create-hold-booking")]
		public async Task<ActionResult<APIResponse>> CreateHoldBooking([FromBody] BookingHoldRequestDTO request)
		{

			var responce = await _unitOfWork.DapperRepo.CreateBookingHoldAsync(request);

			if (responce.Result <= 0)
				return _responseService.InternalServerResponse(responce.Message);

			var hold = responce.Item3;

			hold.PartialDepositAmount = Math.Round(hold.TotalAmount * _partialPaymentPercentage / 100m, 2, MidpointRounding.AwayFromZero);
			hold.PartialBalanceAmount = hold.TotalAmount - hold.PartialDepositAmount;

			return _responseService.SuccessResponse(hold);
		}

		/// <summary>
		/// Initiates a payment request for a booking hold, creating a Stripe PaymentIntent and returning the client secret for front-end processing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("initiate-payment")]
		public async Task<ActionResult<APIResponse>> InitiatePaymentRequest([FromBody] InitiatePaymentRequest request)
		{
			if (request.HoldPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("please enter HoldPublicId");

			var planType = string.IsNullOrWhiteSpace(request.PaymentPlanType) ? "Full" : request.PaymentPlanType;

			if (planType != "Full" && planType != "Partial")
				return _responseService.BadRequestResponse("PaymentPlanType must be 'Full' or 'Partial'");

			decimal? depositPercentage = planType == "Partial" ? _partialPaymentPercentage : null;

			(int Result, string Message, InitiatePaymentResponseDto) response = await _unitOfWork.DapperRepo.InitiatePaymentAsync(request, planType, depositPercentage);

			if (response.Result <= 0)
				return _responseService.InternalServerResponse(response.Message);


			var paymentData = response.Item3;

			// 2. Create Stripe PaymentIntent
			var options = new PaymentIntentCreateOptions
			{
				Amount = checked((long)Math.Round(paymentData.Amount * 100m, MidpointRounding.AwayFromZero)),
				Currency = paymentData.CurrencyCode.ToLowerInvariant(),
				Description = $"YAFA Booking Payment for {paymentData.BookingReference}",
				StatementDescriptorSuffix = "YAFA",
				AutomaticPaymentMethods =
					new PaymentIntentAutomaticPaymentMethodsOptions
					{
						Enabled = true,
						AllowRedirects = "never"
					},

				Metadata = new Dictionary<string, string>
				{
					{ "BookingPublicId", paymentData.BookingPublicId.ToString() },
					{ "PaymentPublicId", paymentData.PaymentPublicId.ToString() },
					{ "BookingReference", paymentData.BookingReference },
					{ "PaymentType", planType == "Partial" ? "Deposit" : "Full" }
				}
			};

			var paymentIntentService = new PaymentIntentService();

			var requestOptions = new RequestOptions
			{
				IdempotencyKey = $"yafa-payment-{paymentData.PaymentPublicId}"
			};

			PaymentIntent paymentIntent;
			try
			{
				paymentIntent = await paymentIntentService.CreateAsync(options, requestOptions);
			}
			catch (StripeException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return _responseService.InternalServerResponse("Unable to initiate payment. Please try again.");
			}


			var updateStripePaymentIntentRequestDTO = new UpdateStripePaymentIntentRequestDTO
			{
				PaymentPublicId = paymentData.PaymentPublicId,
				PaymentIntentId = paymentIntent.Id,
				CustomerId = paymentIntent.CustomerId,
				StripeStatus = paymentIntent.Status,
				ResponseJson = paymentIntent.ToJson()
			};

			var stripeResponse = await _unitOfWork.DapperRepo.UpdateStripePaymentIntentAsync(updateStripePaymentIntentRequestDTO);

			if (stripeResponse.Result <= 0)
				return _responseService.InternalServerResponse(stripeResponse.Message);

			var result = new StripeInitiatePaymentResponseDto
			{
				BookingPublicId = paymentData.BookingPublicId,
				PaymentPublicId = paymentData.PaymentPublicId,
				BookingReference = paymentData.BookingReference,
				Amount = paymentData.Amount,
				CurrencyCode = paymentData.CurrencyCode,
				PaymentIntentId = paymentIntent.Id,
				ClientSecret = paymentIntent.ClientSecret,
				PaymentPlanType = paymentData.PaymentPlanType,
				BookingTotalAmount = paymentData.BookingTotalAmount,
				BookingAmountPaid = paymentData.BookingAmountPaid
			};


			return _responseService.SuccessResponse(result);

			//         var confirmPaymentRequest = new ConfirmPaymentRequest
			//         {
			//             BookingPublicId = responce.Item3.BookingPublicId,
			//             HoldPublicId = request.HoldPublicId,
			//             PaymentMethod = request.PaymentMethod,
			//             PaymentTransactionId = "testpayment1234",
			//             UserId = request.UserId
			//         };

			//         var responce2 = await _unitOfWork.DapperRepo.ConfirmPaymentAsync(confirmPaymentRequest);

			//         if (responce2.Result <= 0)
			//             return _responseService.InternalServerResponse(responce2.Message);

			//_ = SendPaymentNotificationsAsync(request.UserId, responce2.Item3);


		}


		/// <summary>
		/// Initiates the remaining balance payment for a Partial-plan booking whose deposit has already been paid.
		/// Callable any time from deposit confirmation up to (and including) the check-in date.
		/// </summary>
		/// <param name="request">BookingPublicId + UserId identifying the booking to pay off.</param>
		/// <returns>A Stripe PaymentIntent client secret for the remaining balance amount.</returns>
		[HttpPost("pay-balance")]
		public async Task<ActionResult<APIResponse>> InitiateBalancePaymentRequest([FromBody] InitiateBalancePaymentRequest request)
		{
			if (request.BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("please enter BookingPublicId");

			var response = await _unitOfWork.DapperRepo.InitiateBalancePaymentAsync(request);

			if (response.Result <= 0)
				return _responseService.InternalServerResponse(response.Message);

			var paymentData = response.Item3;
			// 2. Create Stripe PaymentIntent
			var options = new PaymentIntentCreateOptions
			{
				Amount = checked((long)Math.Round(paymentData.Amount * 100m, MidpointRounding.AwayFromZero)),
				Currency = paymentData.CurrencyCode.ToLowerInvariant(),
				Description = $"YAFA Balance Payment for {paymentData.BookingReference}",
				StatementDescriptorSuffix = "YAFA",
				AutomaticPaymentMethods =
					new PaymentIntentAutomaticPaymentMethodsOptions
					{
						Enabled = true,
						AllowRedirects = "never"
					},
				Metadata = new Dictionary<string, string>
				{
					{ "BookingPublicId", paymentData.BookingPublicId.ToString() },
					{ "PaymentPublicId", paymentData.PaymentPublicId.ToString() },
					{ "BookingReference", paymentData.BookingReference },
					{ "PaymentType", "Balance" }
				}
			};
			var paymentIntentService = new PaymentIntentService();
			var requestOptions = new RequestOptions
			{
				IdempotencyKey = $"yafa-balance-payment-{paymentData.PaymentPublicId}"
			};
			PaymentIntent paymentIntent;
			try
			{
				paymentIntent = await paymentIntentService.CreateAsync(options, requestOptions);
			}
			catch (StripeException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return _responseService.InternalServerResponse("Unable to initiate balance payment. Please try again.");
			}
			var updateStripePaymentIntentRequestDTO = new UpdateStripePaymentIntentRequestDTO
			{
				PaymentPublicId = paymentData.PaymentPublicId,
				PaymentIntentId = paymentIntent.Id,
				CustomerId = paymentIntent.CustomerId,
				StripeStatus = paymentIntent.Status,
				ResponseJson = paymentIntent.ToJson()
			};
			var stripeResponse = await _unitOfWork.DapperRepo.UpdateStripePaymentIntentAsync(updateStripePaymentIntentRequestDTO);
			if (stripeResponse.Result <= 0)
				return _responseService.InternalServerResponse(stripeResponse.Message);

			var result = new StripeInitiateBalancePaymentResponseDto
			{
				BookingPublicId = paymentData.BookingPublicId,
				PaymentPublicId = paymentData.PaymentPublicId,
				BookingReference = paymentData.BookingReference,
				Amount = paymentData.Amount,
				CurrencyCode = paymentData.CurrencyCode,
				PaymentIntentId = paymentIntent.Id,
				ClientSecret = paymentIntent.ClientSecret,
				BookingTotalAmount = paymentData.BookingTotalAmount,
				BookingAmountPaid = paymentData.BookingAmountPaid
			};

			return _responseService.SuccessResponse(result);
		}

		#endregion

		#region :: Extend Booking

		/// <summary>
		/// Preview an "extend my stay" request — the extra nights, recalculated total (no discount/coupon),
		/// and the extra amount owed. Only for a booking the guest is currently checked in to. Nothing
		/// commits to the booking here — this creates a 5-minute hold, same as create-hold-booking; the
		/// actual extension only applies once initiate-extension-payment's Stripe payment succeeds.
		/// </summary>
		[HttpPost("create-extension-hold")]
		public async Task<ActionResult<APIResponse>> CreateExtensionHold([FromBody] CreateExtensionHoldRequest request)
		{
			if (request.BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("please enter BookingPublicId");

			if (request.NewCheckOutDate == default)
				return _responseService.BadRequestResponse("please enter NewCheckOutDate");

			if (string.IsNullOrWhiteSpace(request.UserId))
				return _responseService.BadRequestResponse("please enter UserId");

			var response = await _unitOfWork.DapperRepo.CreateExtensionHoldAsync(request);

			if (response.Result <= 0)
				return _responseService.InternalServerResponse(response.Message);

			return _responseService.SuccessResponse(response.Item3);
		}

		/// <summary>
		/// The "Pay AED {ExtraAmount}" button on the extend-booking preview screen. Creates a Stripe
		/// PaymentIntent for the extension hold's extra amount; the extension itself (CheckOutDate,
		/// TotalAmount, fees) is only applied to the booking once the webhook confirms this payment.
		/// </summary>
		[HttpPost("initiate-extension-payment")]
		public async Task<ActionResult<APIResponse>> InitiateExtensionPayment([FromBody] InitiateExtensionPaymentRequest request)
		{
			if (request.ExtensionHoldPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("please enter ExtensionHoldPublicId");

			(int Result, string Message, ExtensionPaymentResponseDto) response = await _unitOfWork.DapperRepo.InitiateExtensionPaymentAsync(request);

			if (response.Result <= 0)
				return _responseService.InternalServerResponse(response.Message);

			var paymentData = response.Item3;

			var options = new PaymentIntentCreateOptions
			{
				Amount = checked((long)Math.Round(paymentData.Amount * 100m, MidpointRounding.AwayFromZero)),
				Currency = "INR",
				Description = $"YAFA Extension Payment for {paymentData.BookingReference}",
				StatementDescriptorSuffix = "YAFA",
				AutomaticPaymentMethods =
					new PaymentIntentAutomaticPaymentMethodsOptions
					{
						Enabled = true,
						AllowRedirects = "never"
					},
				Metadata = new Dictionary<string, string>
				{
					{ "BookingPublicId", paymentData.BookingPublicId.ToString() },
					{ "PaymentPublicId", paymentData.PaymentPublicId.ToString() },
					{ "BookingReference", paymentData.BookingReference },
					{ "ExtensionHoldPublicId", paymentData.ExtensionHoldPublicId.ToString() },
					{ "PaymentType", "Extension" }
				}
			};

			var paymentIntentService = new PaymentIntentService();

			var requestOptions = new RequestOptions
			{
				// keyed by PaymentPublicId — a fresh Extension BookingPayments row gets a fresh
				// PaymentPublicId, so retries within the same attempt stay idempotent.
				IdempotencyKey = $"yafa-payment-{paymentData.PaymentPublicId}"
			};

			PaymentIntent paymentIntent;
			try
			{
				paymentIntent = await paymentIntentService.CreateAsync(options, requestOptions);
			}
			catch (StripeException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return _responseService.InternalServerResponse("Unable to initiate extension payment. Please try again.");
			}

			var updateStripePaymentIntentRequestDTO = new UpdateStripePaymentIntentRequestDTO
			{
				PaymentPublicId = paymentData.PaymentPublicId,
				PaymentIntentId = paymentIntent.Id,
				CustomerId = paymentIntent.CustomerId,
				StripeStatus = paymentIntent.Status,
				ResponseJson = paymentIntent.ToJson()
			};

			var stripeResponse = await _unitOfWork.DapperRepo.UpdateStripePaymentIntentAsync(updateStripePaymentIntentRequestDTO);

			if (stripeResponse.Result <= 0)
				return _responseService.InternalServerResponse(stripeResponse.Message);

			var result = new StripeInitiateExtensionPaymentResponseDto
			{
				BookingPublicId = paymentData.BookingPublicId,
				PaymentPublicId = paymentData.PaymentPublicId,
				BookingReference = paymentData.BookingReference,
				Amount = paymentData.Amount,
				CurrencyCode = paymentData.CurrencyCode,
				PaymentIntentId = paymentIntent.Id,
				ClientSecret = paymentIntent.ClientSecret,
				ExtensionHoldPublicId = paymentData.ExtensionHoldPublicId,
				NewCheckOutDate = paymentData.NewCheckOutDate
			};

			return _responseService.SuccessResponse(result);
		}

		#endregion

		#region :: stripe
		/// <summary>
		/// Retrieves the Stripe publishable key from the application configuration for client-side payment processing.
		/// </summary>
		/// <returns></returns>
		[HttpGet("stripe-config")]
		public async Task<ActionResult<APIResponse>> GetStripeConfig()
		{
			var StripePublishableKey = _stripeSettings.PublishableKey;

			if (string.IsNullOrWhiteSpace(StripePublishableKey))
				return _responseService.InternalServerResponse("Stripe publishable key is not configured.");

			return _responseService.SuccessResponse(new { StripePublishableKey });
		}

		/// <summary>
		/// Handles incoming Stripe webhook events for payment processing.
		/// </summary>
		/// <returns></returns>
		[HttpPost("stripe-webhook")]
		[AllowAnonymous]
		public async Task<IActionResult> StripeWebhook()
		{
			try
			{

				using var reader = new StreamReader(Request.Body);
				var json = await reader.ReadToEndAsync();

				var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();


				if (string.IsNullOrWhiteSpace(stripeSignature))
					return BadRequest("Missing Stripe-Signature header");

				var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _stripeSettings.WebhookSecret);

				switch (stripeEvent.Type)
				{
					case EventTypes.PaymentIntentSucceeded:
					case EventTypes.PaymentIntentPaymentFailed:
					case EventTypes.PaymentIntentCanceled:
						break;

					default:
						return Ok();
				}

				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

				if (paymentIntent == null)
				{
					return BadRequest("Invalid PaymentIntent.");
				}

				var webhookRequest = new ProcessStripeWebhookRequestDTO
				{
					StripeEventId = stripeEvent.Id,
					PaymentIntentId = paymentIntent.Id,
					EventType = stripeEvent.Type,
					PaymentIntentStatus = paymentIntent.Status,

					CustomerId = paymentIntent.CustomerId ?? string.Empty,
					PaymentMethodId = paymentIntent.PaymentMethodId ?? string.Empty,
					ChargeId = paymentIntent.LatestChargeId ?? string.Empty,

					Payload = json,
					CardBrand = string.Empty,
					CardLast4 = string.Empty
				};

				//Fetch card brand/last4 for display — the webhook payload only carries the PaymentMethodId
				if (!string.IsNullOrEmpty(paymentIntent.PaymentMethodId))
				{
					try
					{
						var paymentMethod = await new PaymentMethodService().GetAsync(paymentIntent.PaymentMethodId);
						webhookRequest.CardBrand = paymentMethod?.Card?.Brand ?? string.Empty;
						webhookRequest.CardLast4 = paymentMethod?.Card?.Last4 ?? string.Empty;
					}
					catch (StripeException ex)
					{
						await _logService.LogExceptionErrorAsync(ex);
					}
				}

				await _logService.LogCustomAsync($"Stripe webhook received: EventId={stripeEvent.Id}, PaymentIntentId={paymentIntent.Id} ,EventType={stripeEvent.Type}");

				var response = await _unitOfWork.DapperRepo.ProcessStripeWebhookAsync(webhookRequest);

				if (response.Result <= 0)
				{
					await _logService.LogExceptionErrorAsync(new Exception($"Stripe webhook processing failed: " + response.Message));
					return BadRequest(response.Message);
				}

				if (response.Item3.IsNewlyConfirmed && !string.IsNullOrEmpty(response.Item3.NotifyUserId))
				{
					await SendPaymentNotificationsAsync(response.Item3);
				}
				else if (response.Item3.IsPaymentFailed && !string.IsNullOrEmpty(response.Item3.NotifyUserId))
				{
					await SendPaymentFailedNotificationAsync(response.Item3);
				}

				return Ok();
			}
			catch (StripeException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return BadRequest();
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

		}


		/// <summary>
		/// Retrieves the payment status for a specific payment, identified by its public ID and the user's ID. The response includes details about the payment, such as its status, amount, and related booking information.
		/// </summary>
		/// <param name="paymentPublicId"></param>
		/// <param name="userId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet("payment-status")]
		public async Task<ActionResult<APIResponse>> GetPaymentStatus([FromQuery, BindRequired] Guid paymentPublicId, [FromQuery, BindRequired] string userId, CancellationToken cancellationToken)
		{
			if (paymentPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("PaymentPublicId can not be empty");

			if (string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId can not be empty");

			var result = await _unitOfWork.DapperRepo.PaymentResultAsync(paymentPublicId, userId, cancellationToken);

			return _responseService.SuccessResponse(result);
		}

		#endregion


		/// <summary>
		/// Retrieves the unavailable booking dates for a specified property, including confirmed, hold, and blocked dates.
		/// </summary>
		/// <param name="propertyPublicId"></param>
		/// <returns></returns>
		[HttpGet("get-property-unavailable-dates")]
		public async Task<ActionResult<APIResponse>> GetPropertyUnavailableDates([FromQuery, BindRequired] Guid propertyPublicId)
		{
			if (propertyPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyUnavailableDatesAsync(propertyPublicId);

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the property details required for booking, including pricing, availability, and other relevant information.
		/// </summary>
		/// <param name="propPublicId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		[HttpGet("get-property-selectdetails-for-booking")]
		public async Task<ActionResult<APIResponse>> GetPropertySelectDetailsForBooking([FromQuery] Guid propPublicId, [FromQuery, BindRequired] string UserId)
		{
			if (propPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

			var result = await _unitOfWork.DapperRepo.GetPropertySelectDetailsForBookingAsync(propPublicId, UserId);

			if (result is null)
				return _responseService.InternalServerResponse("something went wrong");

			var unavailableDates = await _unitOfWork.DapperRepo.GetPropertyUnavailableDatesAsync(propPublicId);

			result.UnavailableDates = unavailableDates;

			return _responseService.SuccessResponse(result);
		}


		/// <summary>
		/// Retrieves a paginated list of bookings made by a specific user, optionally filtered by a search term.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpGet("get-user-booking-list-item")]
		public async Task<ActionResult<APIResponse>> GetUserBookingListItem([FromQuery, BindRequired] UserBookedDetailRequestDTO request)
		{
			var result = await _unitOfWork.DapperRepo.GetUserBookingListItemAsync(request);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			return _responseService.SuccessResponse(result.Item3);
		}

		/// <summary>
		/// Retrieves the details of a specific booking for a user, identified by the booking's public ID and the user's ID.
		/// </summary>
		/// <param name="BookingPublicId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		[HttpGet("get-booking-details-by-bookingid-and-userid")]
		public async Task<ActionResult<APIResponse>> GetBookingDetailsByBookingIdAndUserId([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery, BindRequired] string UserId)
		{
			if (BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("BookingPublicId can not null or empty");

			if (string.IsNullOrEmpty(UserId))
				return _responseService.BadRequestResponse("UserId can not null or empty");

			var result = await _unitOfWork.DapperRepo.GetBookingDetailsByBookingIdAndUserIdAsync(BookingPublicId, UserId);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			return _responseService.SuccessResponse(result.Item3);
		}

		/// <summary>
		/// User cancels their own booking.
		/// Allowed only when status is Pending, AwaitingPayment or Confirmed.
		/// Refund logic will be added when payment integration starts.
		/// </summary>
		[HttpPost("cancel-booking")]
		public async Task<ActionResult<APIResponse>> CancelBooking([FromBody] UserCancelBookingRequestDto request)
		{
			if (request.BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("BookingPublicId can not null or empty");

			var result = await _unitOfWork.DapperRepo.UserCancelBookingAsync(request);

			_ = SendCancelNotificationsAsync(request.CancelBy, result.OwnerUserId, result.PropertyName, request.BookingPublicId);

			return _responseService.SuccessResponse(result.Message);
		}

		//[HttpPost("extend-booking")]
		//public async Task<ActionResult<APIResponse>> ExtendBooking([FromBody] UserExtendBookingRequestDto request)
		//{
		//	if (request.NewCheckOutDate is null && request.NewNumberOfGuests is null)
		//		return _responseService.BadRequestResponse(
		//			"Please provide a new Check-out date or updated number of guests");

		//	// Parse date if provided
		//	DateOnly? newCheckOut = null;
		//	if (request.NewCheckOutDate is not null)
		//	{
		//		if (!DateOnly.TryParseExact(request.NewCheckOutDate, "yyyy-MM-dd", out var parsed))
		//			return _responseService.BadRequestResponse("Invalid date format. Use yyyy-MM-dd");

		//		newCheckOut = parsed;
		//	}


		//	var result = await _unitOfWork.DapperRepo.UserExtendBookingAsync(request, newCheckOut);

		//	if (result.Result <= 0)
		//		return _responseService.InternalServerResponse(result.Message);

		//	_ = SendExtendNotificationAsync(request.ExtendBy, result.responseDto);

		//	return _responseService.SuccessResponse(result.responseDto);
		//}

		/// <summary>
		/// Allows a user to submit a rating and review for a property they have booked. The request includes the property ID, rating, comment, and booking ID. Upon successful submission, a notification is sent to the property owner.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("user-property-rating-and-review")]
		public async Task<ActionResult<APIResponse>> UserPropertyRatingAndReview([FromBody] PropRatingRequestDTO request)
		{
			var result = await _unitOfWork.DapperRepo.UserRateAndReviewAsync(request);

			_ = SendReviewNotificationAsync(result.OwnerUserId, request.PropertyPublicId);

			return _responseService.SuccessResponse(result.Message);
		}

		/// <summary>
		/// Retrieves a list of available coupons for a specific booking hold and user. The request requires the hold's public ID and the user's ID. The response includes any applicable coupons that can be used for the booking.
		/// </summary>
		/// <param name="holdPublicId"></param>
		/// <param name="userId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet("get-available-coupons")]
		public async Task<ActionResult<APIResponse>> GetAvailableCoupons([FromQuery, BindRequired] Guid holdPublicId, [FromQuery, BindRequired] string userId, CancellationToken cancellationToken)
		{
			if (holdPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("HoldPublicId can not be empty");

			if (string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId can not be empty");

			var result = await _unitOfWork.DapperRepo.GetAvailableCouponsForCheckoutAsync(holdPublicId, userId, cancellationToken);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			if (!result.Item3.Any())
				return _responseService.SuccessResponse("No available coupons found.");

			return _responseService.SuccessResponse(result.Item3);
		}

		/// <summary>
		/// Returns the formal tax invoice for a booking as JSON — company/TRN, guest, stay, itemized
		/// charges with VAT shown separately, and full payment history. The invoice number is
		/// generated once (on first view, by anyone) and is permanent thereafter; every other field
		/// is computed live from current booking data.
		/// </summary>
		[HttpGet("get-invoice")]
		public async Task<ActionResult<APIResponse>> GetInvoice([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery, BindRequired] string UserId)
		{
			if (BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("BookingPublicId can not null or empty");

			if (string.IsNullOrEmpty(UserId))
				return _responseService.BadRequestResponse("UserId can not null or empty");

			var response = await _unitOfWork.DapperRepo.GetOrCreateInvoiceAsync(BookingPublicId, UserId);

			if (response.Result <= 0)
				return _responseService.InternalServerResponse(response.Message);

			return _responseService.SuccessResponse(response.Item3);
		}

		/// <summary>
		/// Same invoice data as get-invoice, rendered as a downloadable PDF.
		/// </summary>
		[HttpGet("download-invoice-pdf")]
		public async Task<IActionResult> DownloadInvoicePdf([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery, BindRequired] string UserId)
		{
			if (BookingPublicId == Guid.Empty || string.IsNullOrWhiteSpace(UserId))
				return BadRequest("BookingPublicId and UserId are required");

			var response = await _unitOfWork.DapperRepo.GetOrCreateInvoiceAsync(BookingPublicId, UserId);

			if (response.Result <= 0)
				return NotFound(response.Message);

			//byte[] pdfBytes;
			try
			{
				var pdfBytes = await _unitOfWork.InvoicePdfService.GenerateInvoicePdfAsync(response.Item3);
				return File(pdfBytes,"application/pdf",$"{response.Item3.InvoiceNumber}.pdf");
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return BadRequest("Unable to generate invoice PDF.");
			}

			//return File(pdfBytes, "application/pdf", $"{response.Item3.InvoiceNumber}.pdf");
		}


		#region :: PRIVATE METHODS 

		private async Task SendPaymentNotificationsAsync(StripeWebhookResponseDto responce)
		{
			try
			{
				var bookingRef = responce.NotifyBookingPublicId;
				var propertyRef = responce.NotifyPropertyPublicId;
				var isBalancePayment = responce.NotifyPaymentType == "Balance";
				var isDeposit = responce.NotifyPaymentType == "Deposit";
				var isExtension = responce.NotifyPaymentType == "Extension";

				string bookingTitle = isExtension ? "Stay Extended" : isBalancePayment ? "Balance Payment Received" : "Booking Confirmed!";

				string bookingBody = isExtension
					? $"Your stay at {responce.NotifyPropertyName} has been extended. New check-out: {responce.NotifyCheckOutDate:dd MMM yyyy}."
					: isBalancePayment
						? $"Your remaining balance for {responce.NotifyPropertyName} has been paid in full."
						: isDeposit
							? $"Your booking at {responce.NotifyPropertyName} is confirmed with a deposit. Balance of {(responce.NotifyTotalAmount - responce.NotifyAmountPaid):N0} AED due before check-in."
							: $"Your booking at {responce.NotifyPropertyName} has been confirmed.";

				string paymentBody = isDeposit
					? $"Deposit of {responce.NotifyPaymentAmount:N0} AED for {responce.NotifyPropertyName} was successful."
					: $"Payment of {responce.NotifyPaymentAmount:N0} AED for {responce.NotifyPropertyName} was successful.";

				// 1. Insert both in-app notifications in parallel
				await Task.WhenAll(
					_unitOfWork.DapperRepo.InsertSingleNotificationAsync(
						responce.NotifyUserId,
						bookingTitle,
						bookingBody,
						NotificationType.Booking,
						bookingRef
					),
					_unitOfWork.DapperRepo.InsertSingleNotificationAsync(
						responce.NotifyUserId,
						"Payment Successful",
						paymentBody,
						NotificationType.Payment,
						bookingRef
					)
				);

				// 2. Push Firebase notification (one push — show booking confirmed)
				var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(responce.NotifyUserId);

				if (deviceToken.Any())
				{
					var pushTitle = isExtension ? "Stay Extended! 🎉" : isBalancePayment ? "Balance Paid! 🎉" : "Booking Confirmed! 🎉";
					var pushBody = isExtension
						? $"Your stay at {responce.NotifyPropertyName} now runs through {responce.NotifyCheckOutDate:dd MMM yyyy}."
						: isBalancePayment
							? $"Your booking at {responce.NotifyPropertyName} is now fully paid."
							: $"Your booking at {responce.NotifyPropertyName} is confirmed. Check-in: {responce.NotifyCheckInDate:dd MMM yyyy}";

					await _firebase.SendBulkAsync(
						deviceToken,
						pushTitle,
						pushBody,
						new Dictionary<string, string>
						{
							{ "type",        NotificationType.Booking },
							{ "referenceId", bookingRef }
						}
					);
				}
			}
			catch (Exception ex)
			{
				// Log silently — user already got payment success response
				await _logService.LogExceptionErrorAsync(ex);
			}
		}

		private async Task SendPaymentFailedNotificationAsync(StripeWebhookResponseDto responce)
		{
			try
			{
				var bookingRef = responce.NotifyBookingPublicId;

				var message = $"Your payment for {responce.NotifyPropertyName} didn't go through. Please start again.";

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					responce.NotifyUserId,
					"Payment Unsuccessful",
					message,
					NotificationType.Payment,
					bookingRef
				);

				var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(responce.NotifyUserId);
				if (deviceToken.Any())
				{
					await _firebase.SendBulkAsync(
						deviceToken,
						"Payment Unsuccessful",
						message,
						new Dictionary<string, string>
						{
							{ "type",        NotificationType.Payment },
							{ "referenceId", bookingRef }
						}
					);
				}
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
			}
		}

		private async Task SendReviewNotificationAsync(string? ownerUserId, Guid propertyPublicId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(ownerUserId)) return;

				var propertyRef = propertyPublicId.ToString();

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					ownerUserId,
					"New Review Received",
					"Someone reviewed your property. Check it out!",
					NotificationType.Review,
					propertyRef
				);

				var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(ownerUserId);

				if (deviceToken.Any())
				{
					await _firebase.SendBulkAsync(
						deviceToken,
						"New Review! ⭐",
						"Someone reviewed your property. Check it out!",
						new Dictionary<string, string>
						{
							{ "type",        NotificationType.Review },
							{ "referenceId", propertyRef }
						}
					);
				}
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
			}
		}

		private async Task SendCancelNotificationsAsync(string userId, string? ownerUserId, string? propertyName, Guid bookingPublicId)
		{
			try
			{
				var bookingRef = bookingPublicId.ToString();
				var propName = propertyName ?? "your villa";

				// Notify user (guest) + owner in parallel
				var tasks = new List<Task>
				{
                    // 1. In-app — guest
                    _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
						userId,
						"Booking Cancelled",
						$"Your booking at {propName} has been cancelled.",
						NotificationType.Booking,
						bookingRef
					),
				};

				// 2. In-app + push — owner
				if (!string.IsNullOrWhiteSpace(ownerUserId))
				{
					tasks.Add(_unitOfWork.DapperRepo.InsertSingleNotificationAsync(
						ownerUserId,
						"Booking Cancelled",
						$"A guest has cancelled their booking at {propName}.",
						NotificationType.Booking,
						bookingRef
					));
				}

				await Task.WhenAll(tasks);

				// 3. Push to guest device
				var guestToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);
				if (guestToken.Any())
				{
					await _firebase.SendBulkAsync(guestToken,
						"Booking Cancelled",
						$"Your booking at {propName} has been cancelled.",
						new Dictionary<string, string>
						{
							{ "type",        NotificationType.Booking },
							{ "referenceId", bookingRef }
						});
				}

				// 4. Push to owner device
				if (!string.IsNullOrWhiteSpace(ownerUserId))
				{
					var ownerToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(ownerUserId);
					if (ownerToken.Any())
					{
						await _firebase.SendBulkAsync(ownerToken,
							"Booking Cancelled",
							$"A guest cancelled their booking at {propName}.",
							new Dictionary<string, string>
							{
								{ "type",        NotificationType.Booking },
								{ "referenceId", bookingRef }
							});
					}
				}
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
			}
		}


		#endregion

	}
}
