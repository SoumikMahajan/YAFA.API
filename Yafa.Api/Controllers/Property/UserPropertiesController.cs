using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;

namespace Yafa.Api.Controllers.Property
{
	/// <summary>
	/// Handles property management and booking lifecycle.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserPropertiesController : ControllerBase
	{
		private readonly ILogService _logService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApiResponseService _responseService;
		private readonly IFileRepo _fileService;
		private readonly IFirebaseService _firebase;

		/// <summary>
		/// Initializes a new instance of the UserPropertiesController class with the specified services required for
		/// user property management operations.
		/// </summary>
		/// <remarks>All parameters must be provided to ensure the controller is properly initialized with
		/// its required dependencies.</remarks>
		/// <param name="logService">The logging service used to record application events and errors.</param>
		/// <param name="unitOfWork">The unit of work service that manages database transactions and coordinates repository operations.</param>
		/// <param name="configuration">The configuration service that provides access to application settings.</param>
		/// <param name="responseService">The service responsible for generating standardized API responses.</param>
		/// <param name="fileService">The file repository service used for file storage and retrieval operations.</param>
		/// <param name="firebase">The Firebase service used to interact with Firebase features such as authentication and database access.</param>
		public UserPropertiesController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService, IFirebaseService firebase)
		{
			_logService = logService;
			_unitOfWork = unitOfWork;
			_responseService = responseService;
			_fileService = fileService;
			_firebase = firebase;
		}

		/// <summary>
		/// Retrieves all properties for developer only.
		/// </summary>
		/// <remarks>This method performs an asynchronous operation to fetch properties from the data
		/// repository. The response will either contain the list of properties or a success message if none are
		/// available.</remarks>
		/// <returns>An APIResponse containing a list of properties for the developer. If no properties are found, the response
		/// includes a message indicating that no properties were found.</returns>
		[HttpGet("get-all-properties-for-developer")]
		public async Task<ActionResult<APIResponse>> GetAllPropertiesForDeveloper()
		{
			var result = await _unitOfWork.DapperRepo.GetAllPropForDeveloper();
			if (!result.Any())
			{
				return _responseService.SuccessResponse("No Property Found!");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves all available amenities.
		/// </summary>
		/// <remarks>This method performs an asynchronous operation to fetch all amenities from the data
		/// store. If no amenities exist, the response will indicate that none were found. Use this endpoint to obtain
		/// the complete set of amenities for client display or selection.</remarks>
		/// <returns>An ActionResult containing an APIResponse with a list of amenities if any are found; otherwise, an error
		/// response indicating that no amenities are available.</returns>
		[HttpGet("get-all-amenities")]
		public async Task<ActionResult<APIResponse>> GetAllAmenities()
		{
			var result = await _unitOfWork.DapperRepo.GetAllAmenitiesAsync();

			if (!result.Any())
			{
				return _responseService.ErrorResponse("No Amenities Found!");
			}

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves all additional services available in the system.
		/// </summary>
		/// <remarks>This method performs an asynchronous operation to fetch additional services from the
		/// database. If no services are found, it returns an error response indicating that no additional services were
		/// found.</remarks>
		/// <returns>An <see langword="ActionResult"/> containing an <see langword="APIResponse"/> with the list of additional
		/// services. Returns an error response if no additional services are found.</returns>
		[HttpGet("get-all-additional-services")]
		public async Task<ActionResult<APIResponse>> GetAdditionalServices()
		{
			var result = await _unitOfWork.DapperRepo.GetAllAdditionalServicesAsync();

			if (!result.Any())
			{
				return _responseService.ErrorResponse("No AdditionalServices Found!");
			}

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves all default house rules from the repository.
		/// </summary>
		/// <remarks>This method performs an asynchronous operation to fetch house rules. If no house
		/// rules are available, it returns an error message indicating that no house rules were found.</remarks>
		/// <returns>An API response containing a list of house rules. Returns an error response if no house rules are found.</returns>
		[HttpGet("get-all-default-house-rules")]
		public async Task<ActionResult<APIResponse>> GetAllHouseRules()
		{
			var result = await _unitOfWork.DapperRepo.GetAllHouseRulesAsync();

			if (!result.Any())
			{
				return _responseService.ErrorResponse("No House Rule Found!");
			}

			return _responseService.SuccessResponse(result);
		}


		/// <summary>
		/// Adds a new property for a user as the first step in the property creation process.
		/// </summary>
		/// <remarks>Logs the outcome of the property addition process. Returns a bad request response if
		/// the addition fails.</remarks>
		/// <param name="request">An object containing the details required to create the property in the first step.</param>
		/// <returns>An ActionResult containing an APIResponse with a success message and the identifier of the newly added
		/// property if the operation succeeds; otherwise, a bad request response with an error message.</returns>
		[HttpPost("add-first-step-property")]
		public async Task<ActionResult<APIResponse>> UserAddPropertyFirstStep([FromBody] UserAddFirstStepPropDTO request)
		{
			var (result, message, propertyId) = await _unitOfWork.DapperRepo.AddPropertyFirstStepAsync(request);

			if (result <= 0)
			{
				await _logService.LogCustomAsync($"AddFirstStep FAILED: {message}", "ERROR");
				return _responseService.BadRequestResponse(message);
			}

			await _logService.LogCustomAsync($"AddFirstStep SUCCESS: PropertyId={propertyId}", "SUCCESS");

			return _responseService.SuccessResponse(new
			{
				Message = message,
				PropertyId = propertyId
			});
		}

		/// <summary>
		/// Handles the second step of adding a user property by processing the provided property details
		/// </summary>
		/// <remarks>This method logs the outcome of the property addition attempt, recording both
		/// successful and failed operations. Ensure that the request object contains all necessary information for the
		/// property to be added.</remarks>
		/// <param name="request">An object containing the details required to complete the second step of property addition, including the
		/// property's public identifier and other relevant information. This parameter cannot be null.</param>
		/// <returns>An ActionResult containing an APIResponse that indicates whether the property was successfully added.
		/// Returns a success response if the operation completes successfully; otherwise, returns a bad request
		/// response with an error message.</returns>
		[HttpPost("add-second-step-property")]
		public async Task<ActionResult<APIResponse>> UserAddPropertySecondStep([FromBody] UserAddSecondStepPropDTO request)
		{
			var (result, message) = await _unitOfWork.DapperRepo.AddPropertySecondStepAsync(request);

			if (result <= 0)
			{
				await _logService.LogCustomAsync($"AddSecondStep FAILED: {message}", "ERROR");
				return _responseService.BadRequestResponse(message);
			}

			await _logService.LogCustomAsync($"AddSecondStep SUCCESS: PropertyId={request.PropertyPublicId}", "SUCCESS");

			return _responseService.SuccessResponse(message);
		}


		/// <summary>
		/// Handles the third step of adding a user property by processing the provided property details
		/// </summary>
		/// <remarks>Logs the outcome of the property addition attempt. Returns a bad request response if
		/// the addition fails, including an error message describing the reason for failure.</remarks>
		/// <param name="request">A data transfer object containing the details required to complete the third step of the property addition
		/// process. Cannot be null.</param>
		/// <returns>An ActionResult containing an APIResponse that indicates whether the property was added successfully or
		/// provides an error message if the operation failed.</returns>
		[HttpPost("add-third-step-property")]
		public async Task<ActionResult<APIResponse>> UserAddPropertyThirdStep([FromBody] UserAddThirdStepPropDTO request)
		{
			var (result, message) = await _unitOfWork.DapperRepo.AddPropertyThirdStepAsync(request);
			if (result <= 0)
			{
				await _logService.LogCustomAsync($"AddThirdStep FAILED: {message}", "ERROR");
				return _responseService.BadRequestResponse(message);
			}

			await _logService.LogCustomAsync($"AddThirdStep SUCCESS: PropertyId={request.PropertyPublicId}", "SUCCESS");

			return _responseService.SuccessResponse(message);
		}

		/// <summary>
		/// Handles the fourth step of the property submission process by uploading property images and finalizing the
		/// property submission.
		/// </summary>
		/// <remarks>This method validates the input request, uploads each provided image, and associates
		/// them with the specified property. If any image upload or database operation fails, an appropriate error
		/// response is returned. The property submission is finalized only if all steps succeed.</remarks>
		/// <param name="request">A data transfer object containing the public identifier of the property and a collection of images to be
		/// uploaded. The property public ID must not be empty.</param>
		/// <returns>An ActionResult containing an APIResponse that indicates whether the operation was successful. The response
		/// includes a message describing the result.</returns>
		[HttpPost("add-fourth-step-property")]
		public async Task<ActionResult<APIResponse>> UserAddPropertyFourthStep([FromBody] UserAddFourthStepPropDTO request)
		{
			if (request is null)
				return _responseService.BadRequestResponse("request can not be null");

			if (request.PropertyPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("Enter PropertyPublicId");

			var imageList = new List<PropertyImageDbDTO>();
			var ImageIndex = 1;

			foreach (var img in request.PropertyImages)
			{
				var imageUploadReult = await _fileService.SaveBase64ImageAsync(
					new Base64ImageSaveRequestDTO
					{
						Base64Image = img.ProfileImageBase64,
						Extension = img.ProfileImageExtension ?? "jpg",
						FileName = $"{request.PropertyPublicId}_PropImage_{ImageIndex}",
						ImageFolderPath = "UploadFiles/PropertyImage",
						ThumbnailFolderPath = "ThumbnailImages/TNPropertyImage",
					}

				);

				if (imageUploadReult == null)
					return _responseService.InternalServerResponse("Image upload failed. Please try again.");

				imageList.Add(new PropertyImageDbDTO
				{
					PropertyImageName = imageUploadReult.FileName,
					PropertyImagePath = imageUploadReult.WebPath,
					PropertyImageLocalPath = imageUploadReult.LocalPath,
					ThumbPropertyImageName = imageUploadReult.ThumbnailImageName,
					ThumbPropertyImagePath = imageUploadReult.ThumbnailWebPath,
					IsCover = ImageIndex == 1,
					DisplayOrder = ImageIndex
				});

				ImageIndex++;
			}

			var jsonImages = JsonConvert.SerializeObject(imageList);

			var (result, message) = await _unitOfWork.DapperRepo.AddPropertyFourthStepAsync(request.PropertyPublicId, jsonImages);

			if (result <= 0)
			{
				await _logService.LogCustomAsync($"AddFourthStep FAILED: {message}", "ERROR");
				return _responseService.InternalServerResponse(message);
			}

			var finalSubmitResult = await _unitOfWork.DapperRepo.FinalSubmitPropertyAsync(request.PropertyPublicId, request.CreatedBy);
			if (finalSubmitResult.Result <= 0)
			{
				await _logService.LogCustomAsync($"AddFourthStep FAILED: {finalSubmitResult.Message}", "ERROR");
				return _responseService.InternalServerResponse(finalSubmitResult.Message);
			}

			_ = SendPropertySubmitNotificationAsync(request.CreatedBy, request.PropertyPublicId);

			await _logService.LogCustomAsync($"AddFourthStep SUCCESS: PropertyId={request.PropertyPublicId}", "SUCCESS");

			return _responseService.SuccessResponse(finalSubmitResult.Message);
		}

		/// <summary>
		/// Retrieves the details of the first step process associated with the specified user.
		/// </summary>
		/// <remarks>This method performs an asynchronous operation to fetch the first step process
		/// details from the database. It returns a bad request response if the 'createdBy' parameter is
		/// invalid.</remarks>
		/// <param name="createdBy">The identifier of the user whose first step process details are being retrieved. This parameter cannot be
		/// null or empty.</param>
		/// <returns>An ActionResult containing an APIResponse with the details of the first step process if found; otherwise, a
		/// success response indicating no record was found.</returns>
		[HttpGet("get-already-added-first-step-process/{createdBy}")]
		public async Task<ActionResult<APIResponse>> GetAlreadyAddedFirstStepProcess(string createdBy)
		{
			if (string.IsNullOrEmpty(createdBy))
				return _responseService.BadRequestResponse("userid can not be null or empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyDetailsFirstStepAsync(createdBy);
			if (result == null || string.IsNullOrEmpty(result.PropertyName))
			{
				return _responseService.SuccessResponse("Previous no record found");
			}

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the details of a property's second step process for a specified property and user.
		/// </summary>
		/// <remarks>If the property has no associated amenities or the property identifier is invalid,
		/// the response indicates that no previous record was found. The method returns a bad request response if the
		/// user identifier is null or empty.</remarks>
		/// <param name="propertyPublicId">The unique identifier of the property for which to retrieve second step process details. Must not be an
		/// empty GUID.</param>
		/// <param name="createdBy">The identifier of the user who created the request. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains an APIResponse with the property
		/// details if found; otherwise, a success response indicating that no previous record was found.</returns>
		[HttpGet("get-already-added-second-step-process/{propertyPublicId}/{createdBy}")]
		public async Task<ActionResult<APIResponse>> GetAlreadyAddedSecondStepProcess(Guid propertyPublicId, string createdBy)
		{
			if (propertyPublicId == Guid.Empty)
				return _responseService.SuccessResponse("Previous no record found");

			if (string.IsNullOrEmpty(createdBy))
				return _responseService.BadRequestResponse("userid can not be null or empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyDetailsSecondStepAsync(propertyPublicId, createdBy);
			if (result == null || !result.Amenities.Any())
			{
				return _responseService.SuccessResponse("Previous no record found");
			}

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the details of the third step process associated with a specified property
		/// </summary>
		/// <remarks>If no third step process is found for the specified property and user, or if the
		/// input parameters are invalid, the response will indicate success with a message or a bad request,
		/// respectively. Ensure that both parameters are valid before calling this method.</remarks>
		/// <param name="propertyPublicId">The unique identifier of the property for which to retrieve third step process details. Must not be an empty
		/// GUID.</param>
		/// <param name="createdBy">The identifier of the user who created the property. This value cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains an APIResponse with the details
		/// of the third step process if found; otherwise, a success response indicating that no previous record was
		/// found or a bad request response if the input parameters are invalid.</returns>
		[HttpGet("get-already-added-third-step-process/{propertyPublicId}/{createdBy}")]
		public async Task<ActionResult<APIResponse>> GetAlreadyAddedThirdStepProcess(Guid propertyPublicId, string createdBy)
		{
			if (propertyPublicId == Guid.Empty)
				return _responseService.SuccessResponse("Previous no record found");

			if (string.IsNullOrEmpty(createdBy))
				return _responseService.BadRequestResponse("userid can not be null or empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyDetailsThirdStepAsync(propertyPublicId, createdBy);
			if (result == null || !result.Rules.Any())
			{
				return _responseService.SuccessResponse("Previous no record found");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the details of the fourth step process for a specified property
		/// </summary>
		/// <remarks>If no records are found for the specified property and user, or if the input
		/// parameters are invalid, the method returns a success or bad request response with an appropriate message.
		/// Ensure that both parameters are valid before calling this method.</remarks>
		/// <param name="propertyPublicId">The unique identifier of the property for which the fourth step process details are being retrieved. Must
		/// not be an empty GUID.</param>
		/// <param name="createdBy">The identifier of the user who created the property. This parameter cannot be null or empty.</param>
		/// <returns>A response containing the details of the fourth step process if found; otherwise, a success response
		/// indicating that no record was found or a bad request response if the input parameters are invalid.</returns>
		[HttpGet("get-already-added-fourth-step-process/{propertyPublicId}/{createdBy}")]
		public async Task<ActionResult<APIResponse>> GetAlreadyAddedFourthStepProcess(Guid propertyPublicId, string createdBy)
		{
			if (propertyPublicId == Guid.Empty)
				return _responseService.SuccessResponse("Previous no record found");

			if (string.IsNullOrEmpty(createdBy))
				return _responseService.BadRequestResponse("userid can not be null or empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyDetailsFourthStepAsync(propertyPublicId, createdBy);
			if (result == null || !result.Images.Any())
			{
				return _responseService.SuccessResponse("Previous no record found");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Searches for available properties that match the specified booking criteria.
		/// </summary>
		/// <remarks>This method is asynchronous and should be awaited. It uses the Dapper repository to
		/// perform the search operation.</remarks>
		/// <param name="request">The search criteria used to filter available properties for booking. This parameter must not be null and
		/// should contain valid search parameters.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>A response containing a list of available properties that meet the search criteria. If no properties are
		/// found, the response indicates success with a message stating that no properties were found.</returns>
		[HttpPost("get-available-all-property-for-booking")]
		public async Task<ActionResult<APIResponse>> GetAvailablePropertyForBookingBySearch([FromBody] PropSearchRequestDTO request, CancellationToken cancellationToken)
		{
			var result = await _unitOfWork.DapperRepo.SearchPropertiesAsync(request, cancellationToken);
			if (!result.Any())
			{
				return _responseService.SuccessResponse("No Property Found!");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the details of a property
		/// </summary>
		/// <remarks>Returns a bad request response if the provided propertyPublicId is an empty GUID. The
		/// response includes property rating details if the property is found.</remarks>
		/// <param name="propertyPublicId">The unique public identifier of the property to retrieve. Must not be an empty GUID.</param>
		/// <param name="userId">The identifier of the user requesting the property details. Used for authorization or tracking purposes.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>An APIResponse containing the property details if found; otherwise, a success response indicating that no
		/// property was found.</returns>
		[HttpGet("get-property-details-by-publicid")]
		public async Task<ActionResult<APIResponse>> GetPropertyDetailsByPublicId([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] string userId, CancellationToken cancellationToken)
		{
			if (propertyPublicId == Guid.Empty)
				return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

			var result = await _unitOfWork.DapperRepo.GetPropertyDetailsByPublicIdAsync(propertyPublicId, userId, cancellationToken);

			if (result == null || string.IsNullOrEmpty(result.PropertyName))
			{
				return _responseService.SuccessResponse("No Property Found!");
			}

			var getPropRatingDetails = await _unitOfWork.DapperRepo.GetPropertyDetailsRatingsByPropPublicIdAsync(propertyPublicId, cancellationToken);

			result.propRatingDetails = getPropRatingDetails;

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves a list of all properties associated with the specified owner.
		/// </summary>
		/// <param name="ownerId">The unique identifier of the owner whose properties are to be retrieved. Cannot be null or empty.</param>
		/// <returns>An ActionResult containing an APIResponse with the list of properties for the specified owner. Returns a
		/// success response with the property list if found, or an appropriate error response if the ownerId is invalid
		/// or no properties are found.</returns>
		[HttpGet("get-all-owner-property-list")]
		public async Task<ActionResult<APIResponse>> GetAllOwnerPropertyList([FromQuery, BindRequired] string ownerId)
		{
			if (string.IsNullOrEmpty(ownerId))
				return _responseService.BadRequestResponse("ownerId can not be null or empty");

			var result = await _unitOfWork.DapperRepo.GetAllOwnerPropertyListAsync(ownerId);

			if (result.Result <= 0)
				return _responseService.InternalServerResponse(result.Message);

			if (!result.Item3.Any())
			{
				return _responseService.SuccessResponse("No Property Found!");
			}

			return _responseService.SuccessResponse(result.Item3);
		}

		/// <summary>
		/// Determines whether there are any popular properties near the current location specified in the request.
		/// </summary>
		/// <param name="req">An object containing the location and criteria used to identify popular properties.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>An ActionResult containing an APIResponse with the list of popular properties near the specified location,
		/// or a message indicating that no properties were found.</returns>
		[HttpPost("is-popular-property-based-on-current-location")]
		public async Task<ActionResult<APIResponse>> IsPopularPropertyBasedOnCurrentLocation([FromBody] IsPopularPropReqDTO req,CancellationToken cancellationToken)
		{
			var result = await _unitOfWork.DapperRepo.IsPopularPropertyBasedOnCurrentLocationAsync(req, cancellationToken);
			if (!result.Any())
			{
				return _responseService.SuccessResponse("No Property Found!");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Determines whether there are any featured properties available based on the specified location criteria.
		/// </summary>
		/// <remarks>Use this endpoint to check for featured properties relevant to the user's current or
		/// specified location. The request must include all necessary location details in the request body.</remarks>
		/// <param name="req">An object containing the location and filtering criteria used to identify featured properties.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>An API response containing a list of featured properties that match the specified criteria. If no properties
		/// are found, the response indicates that no property was found.</returns>
		[HttpPost("is-featured-property-based-on-current-location")]
		public async Task<ActionResult<APIResponse>> IsFeaturedPropertyBasedOnCurrentLocation([FromBody] IsFeaturedPropReqDTO req, CancellationToken cancellationToken)
		{
			var result = await _unitOfWork.DapperRepo.IsFeaturedPropertyBasedOnCurrentLocationAsync(req, cancellationToken);
			if (!result.Any())
			{
				return _responseService.SuccessResponse("No Property Found!");
			}
			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Retrieves the top locations for a specified city.
		/// </summary>
		/// <remarks>This endpoint is accessible via HTTP GET at 'get-top-locations-by-city'. The response
		/// will indicate success, failure, or if no locations are found for the given city.</remarks>
		/// <param name="City">The name of the city for which to retrieve top locations. Cannot be null or empty.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>An asynchronous operation that returns an ActionResult containing an APIResponse with the list of top
		/// locations for the specified city. Returns a success response with the locations if found; otherwise, returns
		/// a response indicating no locations were found or an error occurred.</returns>
		[HttpGet("get-top-locations-by-city")]
		public async Task<ActionResult<APIResponse>> GetTopLocationsByCity([FromQuery, BindRequired] string City, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(City))
				return _responseService.BadRequestResponse("City can not be null or empty");

			var responce = await _unitOfWork.DapperRepo.GetTopLocationsByCityAsync(City, cancellationToken);

			if (responce.Result <= 0)
				return _responseService.InternalServerResponse(responce.Message);

			if (!responce.Item3.Any())
			{
				return _responseService.SuccessResponse("No Location Found!");
			}

			return _responseService.SuccessResponse(responce.Item3);
		}


        #region :: Offer

		/// <summary>
		/// Retrieves the list of available coupons for the specified user to display on the offer page.
		/// </summary>
		/// <param name="userId">The unique identifier of the user for whom to retrieve available coupons. Cannot be null or empty.</param>
		/// <returns>An ActionResult containing an APIResponse with the list of available coupons if found; otherwise, a response
		/// indicating no coupons are available or an error occurred.</returns>
        [HttpGet("get-available-coupons-on-offer-page")]
        public async Task<ActionResult<APIResponse>> GetAvailableCouponsOnOfferPage([FromQuery, BindRequired] string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return _responseService.BadRequestResponse("UserId can not be empty");

            var result = await _unitOfWork.DapperRepo.GetAvailableCouponsForOfferPageAsync(userId);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            if (!result.Item3.Any())
                return _responseService.SuccessResponse("No available coupons found.");

            return _responseService.SuccessResponse(result.Item3);
        }
		#endregion

		#region :: Private Methods

		private async Task SendPropertySubmitNotificationAsync(string ownerUserId, Guid propertyPublicId)
		{
			try
			{
				var propertyRef = propertyPublicId.ToString();

				// In-app only — no urgent push needed for this
				await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
					ownerUserId,
					"Property Submitted!",
					"Your property has been submitted for review. We'll notify you once it's approved.",
					NotificationType.System,
					propertyRef
				);

				var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(ownerUserId);
				if (deviceToken.Any())
				{
					await _firebase.SendBulkAsync(deviceToken,
						"Property Submitted! 🎉",
						"Your property is under review. Approval usually takes 24-48 hours.",
						new Dictionary<string, string>
						{
							{ "type",        NotificationType.System },
							{ "referenceId", propertyRef }
						});
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
