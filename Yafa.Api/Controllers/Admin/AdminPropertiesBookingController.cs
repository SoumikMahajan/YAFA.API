using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;

namespace Yafa.Api.Controllers.Admin
{
    /// <summary>
    /// Provides admin-only endpoints for managing property bookings, including retrieval, status updates,
    /// cancellations, refunds, and extensions.
    /// </summary>
    /// <remarks>All actions require the caller to be authenticated as an admin. Endpoints support booking
    /// lifecycle management for the admin panel, including dashboard data retrieval, status transitions (confirm,
    /// check-in, check-out, cancel), refund processing, and booking extensions. Use this controller to perform
    /// administrative operations on bookings that are not available to regular users.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPropertiesBooking : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;
        private readonly IFileRepo _fileService;
		private readonly IFirebaseService _firebase;

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminPropertiesBooking"/> class.
		/// </summary>
		public AdminPropertiesBooking(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService, IFirebaseService firebase)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
            _fileService = fileService;
			_firebase = firebase;
        }


        #region :: Booked Properties

        /// <summary>
        /// Retrieve a paginated list of all bookings for the admin panel.
        /// </summary>
        /// <remarks>
        /// Supports filtering and pagination via query parameters. Use this as the main
        /// booking dashboard data source.
        /// </remarks>
        /// <param name="request">Query filters such as date range, status, and pagination options.</param>
        /// <returns>A paginated booking list wrapped in <see cref="APIResponse"/>.</returns>
        [HttpGet("admin-panel-get-all-booking-list")]
        public async Task<ActionResult<APIResponse>> AdminPanelGetAllBookingList([FromQuery, BindRequired] AllBookedDetailRequestDTO request)
        {
            var result = await _unitOfWork.DapperRepo.AdminBookingListItemAsync(request);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            return _responseService.SuccessResponse(result.Item3);
        }

        /// <summary>
        /// Retrieve full booking details by booking ID.
        /// </summary>
        /// <remarks>
        /// Returns a single booking record including guest info, dates, property details,
        /// and payment summary.
        /// </remarks>
        /// <param name="BookingPublicId">The unique public GUID of the booking.</param>
        /// <returns>The booking detail object wrapped in <see cref="APIResponse"/>.</returns>
        [HttpGet("admin-panel-booking-details-by-id")]
        public async Task<ActionResult<APIResponse>> AdminPanelBookingDetailsById([FromQuery, BindRequired] Guid BookingPublicId)
        {
            if (BookingPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("BookingPublicId can not be empty");

            var result = await _unitOfWork.DapperRepo.AdminBookingDeatilsByIdAsync(BookingPublicId);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            if (result.Item3 == null)
                return _responseService.InternalServerResponse("No booking details found for the provided BookingPublicId");

            return _responseService.SuccessResponse(result.Item3);
        }

		/// <summary>
		/// Retrieve a list of all booking source types for the admin panel.
		/// </summary>
		/// <returns></returns>
		[HttpGet("admin-panel-list-of-bookings-source-types")]
		public async Task<ActionResult<APIResponse>> AdminPanelListOfBookingsSourceTypes()
		{
			var result = await _unitOfWork.DapperRepo.AdminPanelListOfSourceTypesAsync();		

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieve a price summary for a specific property and date range.
		/// </summary>
		/// <param name="propertyPublicId"></param>
		/// <param name="CheckInDate"></param>
		/// <param name="CheckOutDate"></param>
		/// <returns></returns>
		[HttpGet("admin-panel-booking-price-summery")]
		public async Task<ActionResult<APIResponse>> AdminBookingPriceSummery([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] DateOnly CheckInDate, [FromQuery, BindRequired] DateOnly CheckOutDate)
		{
			if(propertyPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("propertyPublicId can not be empty");

			if(CheckInDate == DateOnly.MinValue)
				return _responseService.BadRequestResponse("CheckInDate can not be empty");

			if(CheckOutDate == DateOnly.MinValue)
				return _responseService.BadRequestResponse("CheckOutDate can not be empty");

			var result = await _unitOfWork.DapperRepo.AdminBookingPriceSummeryAsync(propertyPublicId, CheckInDate, CheckOutDate);

			return _responseService.SuccessResponse(result.Item3);
		}


		/// <summary>
		/// Create a new booking on behalf of a user from the admin panel.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("admin-panel-add-new-booking")]
        public async Task<ActionResult<APIResponse>> AdminPanelAddNewBooking([FromBody] AdminAddNewBookingRequestDTO request)
        {         
            var result = await _unitOfWork.DapperRepo.AdminAddNewBookingAsync(request);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            _ = SendPaymentNotificationsAsync(request.UserId, result.Item3);

            return _responseService.SuccessResponse(result.Message);
        }


        /// <summary>
        /// Confirm a pending booking.
        /// </summary>
        /// <remarks>
        /// Transitions the booking status from **Pending** to **Confirmed**.
        /// This action is irreversible without a subsequent status change.
        /// </remarks>
        /// <param name="BookingPublicId">The public GUID of the booking to confirm.</param>
        /// <param name="CreatedBy">Identifier (username or ID) of the admin performing the action.</param>
        /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
        [HttpPost("admin-panel-confirm-booking")]
        public async Task<ActionResult<APIResponse>> AdminPanelConfirmBooking([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery] string CreatedBy)
        {
            if (BookingPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("BookingPublicId can not be empty");
            if (string.IsNullOrEmpty(CreatedBy))
                return _responseService.BadRequestResponse("CreatedBy can not be empty");

            var result = await _unitOfWork.DapperRepo.AdminPanelConfirmBookingAsync(BookingPublicId, CreatedBy);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            return _responseService.SuccessResponse(result.Message);
        }

        /// <summary>
        /// Update a confirmed booking to Check-In status.
        /// </summary>
        /// <remarks>
        /// Records the guest's arrival and transitions the booking to **Checked-In**.
        /// </remarks>
        /// <param name="BookingPublicId">The public GUID of the booking.</param>
        /// <param name="CreatedBy">Identifier of the admin performing the action.</param>
        /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
        [HttpPost("admin-panel-update-booking-to-check-in")]
        public async Task<ActionResult<APIResponse>> AdminPanelUpdateBookingToCheckIn([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery, BindRequired] string CreatedBy)
        {
            if (BookingPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("BookingPublicId can not be empty");
            if (string.IsNullOrEmpty(CreatedBy))
                return _responseService.BadRequestResponse("CreatedBy can not be empty");

            var result = await _unitOfWork.DapperRepo.AdminPanelUpdateBookingToCheckInAsync(BookingPublicId, CreatedBy);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

			_ = SendCheckInNotificationAsync(result.UserId, result.PropertyName, BookingPublicId);

			return _responseService.SuccessResponse(result.Message);
        }

		/// <summary>
		/// Retrieve check-out details for a specific booking.
		/// </summary>
		/// <param name="BookingPublicId"></param>
		/// <returns></returns>
		[HttpGet("admin-panel-get-check-out-details")]
		public async Task<ActionResult<APIResponse>> AdminPanelGetCheckOutDetails([FromQuery, BindRequired] Guid BookingPublicId)
		{
			if (BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("BookingPublicId can not be empty");		

			var result = await _unitOfWork.DapperRepo.AdminGetCheckOutDetailsAsync(BookingPublicId);


			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Add an extra fee to an existing booking.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("admin-panel-add-extra-fee")]
		public async Task<ActionResult<APIResponse>> AdminPanelAddExtraFee([FromBody] AddExtraFeeRequestDto request)
		{
			var result = await _unitOfWork.DapperRepo.AdminAddExtraFeeAsync(request);

            if(result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			return _responseService.SuccessResponse(result.Message);    
		}

		/// <summary>
		/// Remove an extra fee from an existing booking.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("admin-panel-remove-extra-fee")]
		public async Task<ActionResult<APIResponse>> AdminPanelRemoveExtraFee([FromBody] RemoveExtraFeeRequestDto request)
		{
			var result = await _unitOfWork.DapperRepo.AdminRemoveExtraFeeAsync(request);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			return _responseService.SuccessResponse(result.Message);
		}

		/// <summary>
		/// Add a manual payment to an existing booking.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("admin-panel-add-manual-payment")]
		public async Task<ActionResult<APIResponse>> AdminAddManualPayment([FromBody] AddManualPaymentRequestDto request)
		{
			if(request.BookingPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("BookingPublicId can not be empty");
			if (request.Amount <= 0)
				return _responseService.BadRequestResponse("Amount must be greater than zero");

			if(string.IsNullOrEmpty(request.PaymentMethod))
				return _responseService.BadRequestResponse("PaymentMethod can not be empty");
			if(string.IsNullOrEmpty(request.CreatedBy))
				return _responseService.BadRequestResponse("CreatedBy can not be empty");

			var result = await _unitOfWork.DapperRepo.AdminAddManualPaymentAsync(request);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			return _responseService.SuccessResponse(result.Message);

		}


		/// <summary>
		/// Update a checked-in booking to Check-Out status.
		/// </summary>
		/// <remarks>
		/// Records the guest's departure and transitions the booking to **Checked-Out**.
		/// </remarks>
		/// <param name="BookingPublicId">The public GUID of the booking.</param>
		/// <param name="CreatedBy">Identifier of the admin performing the action.</param>
		/// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
		[HttpPost("admin-panel-update-booking-to-check-out")]
        public async Task<ActionResult<APIResponse>> AdminPanelUpdateBookingToCheckOut([FromQuery, BindRequired] Guid BookingPublicId, [FromQuery, BindRequired] string CreatedBy)
        {
            if (BookingPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("BookingPublicId can not be empty");
            if (string.IsNullOrEmpty(CreatedBy))
                return _responseService.BadRequestResponse("CreatedBy can not be empty");

            var result = await _unitOfWork.DapperRepo.AdminPanelUpdateBookingToCheckOutAsync(BookingPublicId, CreatedBy);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

			_ = SendCheckOutNotificationAsync(result.UserId, result.PropertyName, BookingPublicId);

			return _responseService.SuccessResponse(result.Message);
        }

        /// <summary>
        /// Cancel a booking.
        /// </summary>
        /// <remarks>
        /// Cancels the booking and optionally records a cancellation reason.
        /// If a refund is applicable, use the **Process Refund** endpoint afterward.       
        /// </remarks>
        /// <param name="req">Cancellation request payload.</param>
        /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
        [HttpPost("admin-panel-update-booking-to-cancel-booking")]
        public async Task<ActionResult<APIResponse>> AdminPanelUpdateBookingToCancelBooking([FromBody] CancelBookingRequestDTO req)
        {

            var result = await _unitOfWork.DapperRepo.AdminPanelUpdateBookingToCancelBookingAsync(req);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

			_ = SendAdminCancelNotificationAsync(result.UserId, result.PropertyName, req.BookingPublicId);

			return _responseService.SuccessResponse(result.Message);
        }

        /// <summary>
        /// Process a refund for a cancelled booking.
        /// </summary>
        /// <remarks>
        /// Records the refund against a cancelled booking. This does **not** trigger
        /// a payment gateway call — it updates the booking's financial status in the system.       
        /// </remarks>
        /// <param name="req">Refund request payload including the amount and processor identity.</param>
        /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
        [HttpPost("admin-panel-update-booking-to-process-refund")]
        public async Task<ActionResult<APIResponse>> AdminPanelUpdateBookingToProcessRefund([FromBody] RefundBookingRequestDTO req)
        {
            var result = await _unitOfWork.DapperRepo.AdminPanelUpdateBookingToProcessRefundAsync(req);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            return _responseService.SuccessResponse(result.Message);
        }

        /// <summary>
        /// Extend the checkout date of an active booking.
        /// </summary>
        /// <remarks>
        /// Prolongs the guest's stay by updating the booking end date.
        /// The new end date must not overlap with another confirmed booking for the same property.    
        /// </remarks>
        /// <param name="req">Extension request payload with new checkout date and admin identifier.</param>
        /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
        [HttpPost("admin-panel-extend-booking")]
        public async Task<ActionResult<APIResponse>> AdminPanelExtendBooking([FromBody] ExtendBookingRequestDTO req)
        {
            var result = await _unitOfWork.DapperRepo.AdminPanelExtendBookingAsync(req);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

			_ = SendExtendNotificationAsync(result.UserId, result.PropertyName, result.NewCheckOutDate, req.BookingPublicId);

			return _responseService.SuccessResponse(result.Message);
        }


        /// <summary>
        /// Retrieve a list of all users for creating a new property booking in the admin panel.
        /// </summary>
        /// <remarks>
        /// Used by the admin UI to populate user selection when creating a booking on behalf of a user.
        /// </remarks>
        /// <returns>A list of users wrapped in <see cref="APIResponse"/>.</returns>
        [HttpGet("get-alluser-list-fornew-propbooking-adminpanel")]
        public async Task<ActionResult<APIResponse>> GetAllUserListForNewPropBooking()
        {
            var result = await _unitOfWork.DapperRepo.GetUserListForAdminPanelAsync();
            return _responseService.SuccessResponse(result);
        }

		/// <summary>
		/// Admin Download the invoice PDF for a specific booking.
		/// </summary>
		/// <param name="BookingPublicId"></param>
        /// <param name="IssueBy"></param>
		/// <returns></returns>
		[HttpGet("admin-download-invoice-pdf")]
		public async Task<IActionResult> AdminDownloadInvoicePdf([FromQuery, BindRequired] Guid BookingPublicId,[FromQuery, BindRequired] string IssueBy)
		{
			if (BookingPublicId == Guid.Empty || string.IsNullOrWhiteSpace(IssueBy))
				return BadRequest("BookingPublicId and IssueBy are required");

			var response = await _unitOfWork.DapperRepo.AdminGetOrCreateInvoiceAsync(BookingPublicId, IssueBy);

			if (response.Result <= 0)
				return NotFound(response.Message);

			//byte[] pdfBytes;
			try
			{
				var pdfBytes = await _unitOfWork.InvoicePdfService.GenerateInvoicePdfAsync(response.Item3);
				return File(pdfBytes, "application/pdf", $"{response.Item3.InvoiceNumber}.pdf");
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				return BadRequest("Unable to generate invoice PDF.");
			}

			//return File(pdfBytes, "application/pdf", $"{response.Item3.InvoiceNumber}.pdf");
		}



		#endregion

		#region :: Private Methods

		private async Task SendCheckInNotificationAsync(string? userId, string? propertyName, Guid bookingPublicId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(userId)) return;

				var bookingRef = bookingPublicId.ToString();
				var propName = propertyName ?? "your villa";

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					userId,
					"Welcome! You're Checked In",
					$"Your check-in at {propName} has been confirmed. Enjoy your stay!",
					NotificationType.CheckInReminder,
					bookingRef
				);

				var token = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);
				if (token.Any())
				{
					await _firebase.SendBulkAsync(token,
						"You're Checked In! 🏡",
						$"Welcome to {propName}. Enjoy your stay!",
						new Dictionary<string, string>
						{
					        { "type",        NotificationType.CheckInReminder },
					        { "referenceId", bookingRef }
						});
				}
			}
			catch (Exception ex) { await _logService.LogExceptionErrorAsync(ex); }
		}

		private async Task SendCheckOutNotificationAsync(string? userId, string? propertyName, Guid bookingPublicId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(userId)) return;

				var bookingRef = bookingPublicId.ToString();
				var propName = propertyName ?? "your villa";

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					userId,
					"Checked Out Successfully",
					$"Your stay at {propName} is complete. Hope you enjoyed it! Leave a review.",
					NotificationType.CheckOutReminder,
					bookingRef
				);

				var token = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);
				if (token.Any())
				{
					await _firebase.SendBulkAsync(token,
						"Checked Out 🧳",
						$"Thanks for staying at {propName}! Share your experience with a review.",
						new Dictionary<string, string>
						{
					        { "type",        NotificationType.CheckOutReminder },
					        { "referenceId", bookingRef }
						});
				}
			}
			catch (Exception ex) { await _logService.LogExceptionErrorAsync(ex); }
		}

		private async Task SendAdminCancelNotificationAsync(string? userId, string? propertyName, Guid bookingPublicId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(userId)) return;

				var bookingRef = bookingPublicId.ToString();
				var propName = propertyName ?? "your villa";

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					userId,
					"Booking Cancelled",
					$"Your booking at {propName} has been cancelled by admin. Contact support for details.",
					NotificationType.Booking,
					bookingRef
				);

				var token = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);
				if (token.Any())
				{
					await _firebase.SendBulkAsync(token,
						"Booking Cancelled",
						$"Your booking at {propName} was cancelled. Contact support for help.",
						new Dictionary<string, string>
						{
					        { "type",        NotificationType.Booking },
					        { "referenceId", bookingRef }
						});
				}
			}
			catch (Exception ex) { await _logService.LogExceptionErrorAsync(ex); }
		}

		private async Task SendExtendNotificationAsync(string? userId, string? propertyName,DateOnly? newCheckOut, Guid bookingPublicId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(userId)) return;

				var bookingRef = bookingPublicId.ToString();
				var propName = propertyName ?? "your villa";
				var checkOutStr = newCheckOut.HasValue ? newCheckOut.Value.ToString("dd MMM yyyy") : "updated date";

				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					userId,
					"Booking Extended",
					$"Your stay at {propName} has been extended. New check-out: {checkOutStr}.",
					NotificationType.Booking,
					bookingRef
				);

				var token = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);
				if (token.Any())
				{
					await _firebase.SendBulkAsync(token,
						"Booking Extended! 🏡",
						$"Great news! Your stay at {propName} extended to {checkOutStr}.",
						new Dictionary<string, string>
						{
					        { "type",        NotificationType.Booking },
					        { "referenceId", bookingRef }
						});
				}
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
			}
		}

        private async Task SendPaymentNotificationsAsync(string userId, AdminAddNewBookingResponseDto booking)
        {
            try
            {
                var bookingRef = booking.BookingPublicId.ToString();
                var propertyRef = booking.PropertyPublicId.ToString();

                // 1. Insert both in-app notifications in parallel
                await Task.WhenAll(
                    _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
                        userId,
                        "Booking Confirmed!",
                        $"Your booking at {booking.PropertyName} has been confirmed.",
                        NotificationType.Booking,
                        bookingRef
                    ),
                    _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
                        userId,
                        "Payment Successful",
                        $"Payment of {booking.TotalAmount:N0} AED for {booking.PropertyName} was successful.",
                        NotificationType.Payment,
                        bookingRef
                    )
                );

                // 2. Push Firebase notification (one push — show booking confirmed)
                var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(userId);

                if (deviceToken.Any())
                {
                    await _firebase.SendBulkAsync(
                        deviceToken,
                        "Booking Confirmed! 🎉",
                        $"Your booking at {booking.PropertyName} is confirmed. Check-in: {booking.CheckInDate}",
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
                await _logService.LogExceptionErrorAsync(ex);
            }
        }

        #endregion


    }
}
