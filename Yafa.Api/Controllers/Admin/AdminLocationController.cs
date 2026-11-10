using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Api.Controllers.Admin
{
    /// <summary>
    /// Provides administrative endpoints for managing top-level location entities, including creation, update,
    /// deletion, and status toggling operations.
    /// </summary>
    /// <remarks>This controller is intended for administrative use and exposes APIs to add, update, delete,
    /// and toggle the active status of locations. Most actions require appropriate permissions and log both successful
    /// and failed operations. Profile images associated with locations can be uploaded and managed through these
    /// endpoints. Ensure that callers have the necessary authorization before invoking these actions.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLocationController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;
        private readonly IFileRepo _fileService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logService"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        /// <param name="responseService"></param>
        /// <param name="fileService"></param>
		public AdminLocationController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
            _fileService = fileService;
        }

        #region :: Top Location Management

        /// <summary>
        /// Adds a new top-level location using the provided request data.
        /// </summary>
        /// <remarks>This endpoint is intended for administrative use to add new locations. If a profile
        /// image is provided in the request, it will be saved and associated with the new location. The response
        /// indicates success or failure, along with a relevant message.</remarks>
        /// <param name="request">An object containing the details required to create the new location, including profile image data and
        /// location attributes. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ActionResult with an
        /// APIResponse indicating the outcome of the add operation.</returns>
        [HttpPost("admin-add-location")]
        public async Task<ActionResult<APIResponse>> AddLocation([FromBody] AddTopLocationRequestDto request)
        {

            var LocationPublicId = Guid.NewGuid();

            var imageUploadReult = new ImageUploadResultDTO();
            if (!string.IsNullOrEmpty(request.ProfileImageBase64))
            {
                imageUploadReult = await _fileService.SaveBase64ImageAsync(
                    new Base64ImageSaveRequestDTO
                    {
                        Base64Image = request.ProfileImageBase64,
                        Extension = request.ProfileImageExtension ?? "jpg",
                        FileName = $"{LocationPublicId}_LocImage_1",
                        ImageFolderPath = "UploadFiles/Location",
                        ThumbnailFolderPath = "ThumbnailImages/TNLocation",
                    }
                );

                if (imageUploadReult == null)
                    return _responseService.InternalServerResponse("Image upload failed. Please try again.");
            }

            var (result, message) = await _unitOfWork.DapperRepo.AdminAddLocationAsync(LocationPublicId, imageUploadReult, request);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"Add Location FAILED: {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync($"Add Location SUCCESS: LocationPublicId={LocationPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>
        /// Updates the details of a location using the provided request data. This operation is intended for
        /// administrative use.
        /// </summary>
        /// <remarks>If a profile image is included in the request, it will be processed and saved before
        /// updating the location details. This endpoint is typically restricted to administrative users.</remarks>
        /// <param name="request">An object containing the updated location information, including optional profile image data. Cannot be
        /// null.</param>
        /// <returns>An ActionResult containing an APIResponse that indicates the outcome of the update operation. Returns a
        /// success response if the update is completed; otherwise, returns an error response with details.</returns>
        [HttpPost("admin-update-location")]
        public async Task<ActionResult<APIResponse>> UpdateLocation([FromBody] UpdateTopLocationRequestDto request)
        {
            var imageUploadReult = new ImageUploadResultDTO();
            if (!string.IsNullOrEmpty(request.ProfileImageBase64))
            {
                imageUploadReult = await _fileService.SaveBase64ImageAsync(
                    new Base64ImageSaveRequestDTO
                    {
                        Base64Image = request.ProfileImageBase64,
                        Extension = request.ProfileImageExtension ?? "jpg",
                        FileName = $"{request.LocationPublicId}_LocImage_1",
                        ImageFolderPath = "UploadFiles/Location",
                        ThumbnailFolderPath = "ThumbnailImages/TNLocation",
                    }
                );

                if (imageUploadReult == null)
                    return _responseService.InternalServerResponse("Image upload failed. Please try again.");
            }

            var (result, message) = await _unitOfWork.DapperRepo.AdminUpdateLocationAsync(imageUploadReult, request);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"Update Location FAILED: {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync($"Update Location SUCCESS: LocationPublicId={request.LocationPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>
        /// Deletes a location identified by its public ID and records the user who performed the deletion.
        /// </summary>
        /// <remarks>This action is intended for administrative use. The method logs both successful and
        /// failed deletion attempts. Ensure that the caller has appropriate permissions before invoking this
        /// action.</remarks>
        /// <param name="LocationPublicId">The unique identifier of the location to delete. Must not be an empty GUID.</param>
        /// <param name="UpdatedBy">The username or identifier of the user performing the deletion. Cannot be null or empty.</param>
        /// <returns>An API response indicating the result of the delete operation. Returns a success response if the location is
        /// deleted; otherwise, returns an error response with details.</returns>
        [HttpPost("admin-delete-location")]
        public async Task<ActionResult<APIResponse>> DeleteLocation([FromQuery, BindRequired] Guid LocationPublicId, [FromQuery, BindRequired] string UpdatedBy)
        {
            if (LocationPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid LocationPublicId is required.");

            if (string.IsNullOrEmpty(UpdatedBy))
                return _responseService.BadRequestResponse("UpdatedBy is required.");

            var (result, message) = await _unitOfWork.DapperRepo.AdminDeleteLocationAsync(LocationPublicId, UpdatedBy);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"Delete Location FAILED: {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync($"Delete Location SUCCESS: LocationPublicId={LocationPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>
        /// Toggles the active status of a location identified by its public ID.
        /// </summary>
        /// <param name="LocationPublicId">The unique identifier of the location to update. Must not be an empty GUID.</param>
        /// <param name="UpdatedBy">The username or identifier of the user performing the update. Cannot be null or empty.</param>
        /// <returns>An API response indicating the result of the operation. Returns a success response if the location's status
        /// is toggled; otherwise, returns an error response with details.</returns>
        [HttpPost("admin-location-toggle-active")]
        public async Task<ActionResult<APIResponse>> LocationToggleActive([FromQuery, BindRequired] Guid LocationPublicId, [FromQuery, BindRequired] string UpdatedBy)
        {
            if (LocationPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid LocationPublicId is required.");

            if (string.IsNullOrEmpty(UpdatedBy))
                return _responseService.BadRequestResponse("UpdatedBy is required.");

            var (result, message) = await _unitOfWork.DapperRepo.AdminToggleActiveAsync(LocationPublicId, UpdatedBy);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"Toggle Location Active FAILED: {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync($"Toggle Location Active SUCCESS: LocationPublicId={LocationPublicId}", "SUCCESS");
            return _responseService.SuccessResponse(message);
        }

        /// <summary>
        /// Retrieves all location records for administrative purposes.
        /// </summary>
        /// <remarks>This endpoint is intended for administrative use and returns all available location
        /// data. The response format follows the standard API response structure used throughout the
        /// application.</remarks>
        /// <returns>An asynchronous operation that returns an <see cref="ActionResult{APIResponse}"/> containing the list of all
        /// locations if found; otherwise, a response indicating no locations were found or an internal server error.</returns>
        [HttpGet("admin-get-all-locations")]
        public async Task<ActionResult<APIResponse>> AdminGetAllLocations([FromQuery, BindRequired] int PageNumber, [FromQuery, BindRequired] int PageSize)
        {          
            var responce = await _unitOfWork.DapperRepo.AdminGetAllLocationsAsync(PageNumber,PageSize);

            if (responce.Result <= 0)
            {             
                return _responseService.InternalServerResponse(responce.Message);
            }

            if(!responce.Item3.Any())
            {
                return _responseService.SuccessResponse("No locations found.");
            }

            return _responseService.SuccessResponse(responce.Item3);
        }

        #endregion


    }
}
