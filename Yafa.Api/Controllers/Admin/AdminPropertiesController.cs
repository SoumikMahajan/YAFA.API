using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Yafa.Api.Extension;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.Property;

namespace Yafa.Api.Controllers.Admin;

/// <summary>
/// Handles admin-level operations for property management and booking lifecycle.
/// All endpoints require the <c>Admin</c> role.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminPropertiesController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiResponseService _responseService;
    private readonly IFileRepo _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPropertiesController"/> class.
    /// </summary>
    public AdminPropertiesController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService)
    {
        _logService = logService;
        _unitOfWork = unitOfWork;
        _responseService = responseService;
        _fileService = fileService;
    }

    #region :: Properties

    /// <summary>
    /// Block property dates (make unavailable for booking)
    /// </summary>
    /// <param name="request">Blocking request details</param>
    /// <returns>Success message</returns>
    /// <response code="200">Dates blocked successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("block-property-dates")]
    public async Task<ActionResult<APIResponse>> BlockPropertyDates([FromBody] BlockPropRequestDTO request)
    {
        var (result, message) = await _unitOfWork.DapperRepo.AddBlockPropertyDatesAsync(request);
        if (result <= 0)
        {
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }

    /// <summary>
    /// Unblock previously blocked dates for a property.
    /// </summary>
    /// <remarks>
    /// Removes a specific unavailability record, making those dates bookable again.
    /// </remarks>
    /// <param name="UnavailablePublicId">The unique identifier of the blocked-date record to remove.</param>
    /// <returns>A confirmation message wrapped in <see cref="APIResponse"/>.</returns>
    [HttpPost("unblock-property-dates")]
    public async Task<ActionResult<APIResponse>> UnBlockPropertyDates([FromQuery, BindRequired] Guid UnavailablePublicId)
    {
        var (result, message) = await _unitOfWork.DapperRepo.UnBlockPropertyDatesDatesAsync(UnavailablePublicId);
        if (result <= 0)
        {
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }

    /// <summary>
    /// Retrieve all blocked dates for a specific property.
    /// </summary>
    /// <remarks>
    /// Returns the full list of unavailability windows currently set for the property.
    /// </remarks>
    /// <param name="PropertyPublicId">The public GUID of the target property.</param>
    /// <returns>A list of blocked-date records wrapped in <see cref="APIResponse"/>.</returns>
    [HttpGet("get-property-blocked-dates/{PropertyPublicId}")]
    public async Task<ActionResult<APIResponse>> GetPropertyBlockedDates(Guid PropertyPublicId)
    {
        if (PropertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.BlockPropDatesDetailsAsync(PropertyPublicId);

        return _responseService.SuccessResponse(result);
    }

    /// <summary>
    /// Retrieve all final-submit properties.
    /// </summary>
    /// <remarks>
    /// Returns properties that have been fully submitted by owners.
    /// </remarks>
    /// <returns>A list of final-submitted properties wrapped in <see cref="APIResponse"/>.</returns>
    [HttpGet("get-all-final-submit-properties")]
    public async Task<ActionResult<APIResponse>> GetAllFinalSubmitProperties([FromQuery, BindRequired] AdminPropertySearchRequestDTO request)
    {
        if(request is null)
			return _responseService.BadRequestResponse("Request body cannot be null");

		IEnumerable<FinalSubmitPropertyResponceDTO>? result = await _unitOfWork.DapperRepo.GetAllFinalSubmitPropertiesAsync(request);

        return _responseService.SuccessResponse(result);
    }

    /// <summary>
    /// Updates all details of a property using the provided data transfer object.
    /// </summary>
    /// <param name="req">An object containing the updated property information. Must not be null.</param>
    /// <returns>An ActionResult containing an APIResponse that indicates the outcome of the update operation. Returns a
    /// success response if the update is successful; otherwise, returns an internal server error response.</returns>
    [HttpPost("admin-update-full-property")]
    public async Task<ActionResult<APIResponse>> UpdateFullPropertyAsync([FromBody] AdminUpdateFullPropertyDTO req)
    {

        if (req.PropertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.UpdateFullPropertyAsync(req);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);

    }

    /// <summary>
    /// Retrieves all images associated with a specific property identified by its public identifier.
    /// </summary>
    /// <param name="PropertyPublicId">The public GUID of the target property.</param>
    /// <returns>A list of property images wrapped in <see cref="APIResponse"/>.</returns>
    [HttpGet("admin-get-all-property-images")]
    public async Task<ActionResult<APIResponse>> GetAllPropertyImagesAsync([FromQuery, BindRequired] Guid PropertyPublicId)
    {
        if (PropertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.AdminGetAllPropertyImagesAsync(PropertyPublicId);

        return _responseService.SuccessResponse(result);

    }

    /// <summary>
    /// Deletes a specific property image identified by its public identifier. The image will be removed from the property and deleted from storage.
    /// </summary>
    /// <param name="ImagePublicId"></param>
    /// <param name="DeletedBy"></param>
    /// <returns></returns>
    [HttpPost("admin-delete-property-image")]
    public async Task<ActionResult<APIResponse>> DeletePropertyImageAsync([FromQuery, BindRequired] Guid ImagePublicId, [FromQuery, BindRequired] string DeletedBy)
    {
        if (ImagePublicId == Guid.Empty)
            return _responseService.BadRequestResponse("ImagePublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.AdminDeletePropertyImageAsync(ImagePublicId, DeletedBy);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);
    }

    /// <summary>
    /// Adds new images to a property identified by its public identifier. The images are uploaded, processed, and associated with the property.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("admin-add-property-images")]
    public async Task<ActionResult<APIResponse>> AddPropertyImageAsync([FromBody] AdminAddPropertyImagesDTO request)
    {

        if (request.PropertyImages.Count > 5)
            return _responseService.BadRequestResponse("Max 5 images per upload.");

        var imageList = new List<PropertyImageDbDTO>();

        var latestImagesDisplayOrder = await _unitOfWork.DapperRepo.GetLatestPropertyImageDisplayOrderAsync(request.PropertyPublicId);

        var ImageIndex = latestImagesDisplayOrder.LatestDisplayOrder + 1;
        var failedImages = new List<string>();
        var savedPaths = new List<string>();

        for (int i = 0; i < request.PropertyImages.Count; i++)
        {
            var img = request.PropertyImages[i];

            try
            {
                var base64Data = img.ProfileImageBase64.Contains(",") ? img.ProfileImageBase64.Split(',')[1] : img.ProfileImageBase64;

                var bytes = Convert.FromBase64String(base64Data);
                var extension = ImageHelper.DetectImageExtension(bytes) ?? throw new InvalidOperationException("Unsupported format. Only JPEG, JPG, PNG, WebP allowed.");

                var fileName = $"{request.PropertyPublicId}_PropImage_{ImageIndex}";

                var uploadResult = await _fileService.SaveBase64ImageAsync(new Base64ImageSaveRequestDTO
                {
                    Base64Image = img.ProfileImageBase64,
                    Extension = extension,
                    FileName = fileName,
                    ImageFolderPath = "UploadFiles/PropertyImage",
                    ThumbnailFolderPath = "ThumbnailImages/TNPropertyImage"
                });

                savedPaths.Add(uploadResult.LocalPath);

                imageList.Add(new PropertyImageDbDTO
                {
                    PropertyImageName = uploadResult.FileName,
                    PropertyImagePath = uploadResult.WebPath,
                    PropertyImageLocalPath = uploadResult.LocalPath,
                    ThumbPropertyImageName = uploadResult.ThumbnailImageName,
                    ThumbPropertyImagePath = uploadResult.ThumbnailWebPath,
                    IsCover = ImageIndex == 1,
                    DisplayOrder = ImageIndex
                });
                ImageIndex++;
            }
            catch (Exception ex)
            {
                failedImages.Add($"Image[{i + 1}]: {ex.Message}");
            }
        }

        if (!imageList.Any())
        {
            await _logService.LogCustomAsync(
                $"[AddPropertyImages] ALL FAILED — PropertyId={request.PropertyPublicId} | {string.Join(" | ", failedImages)}",
                "ERROR");

            return _responseService.InternalServerResponse($"All images failed. Errors: {string.Join(" | ", failedImages)}");
        }

        var jsonImages = JsonConvert.SerializeObject(imageList);
        var (result, message) = await _unitOfWork.DapperRepo.AddNewPropertyImagesAsync(request.PropertyPublicId, request.CreatedBy, jsonImages);

        if (result <= 0)
        {
            // Rollback: delete files saved in STEP 1 to prevent orphaned files
            foreach (var path in savedPaths)
            {
                try { if (System.IO.File.Exists(path)) System.IO.File.Delete(path); }
                catch (Exception cleanupEx)
                {
                    await _logService.LogCustomAsync($"[AddPropertyImages] Cleanup failed — {path}: {cleanupEx.Message}", "WARN");
                }
            }

            await _logService.LogCustomAsync(
                $"[AddPropertyImages] DB FAILED — {message} | PropertyId={request.PropertyPublicId}",
                "ERROR");

            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(new
        {
            Saved = imageList.Count,
            Failed = failedImages.Count,
            FailedErrors = failedImages.Any() ? failedImages : null,
            Message = failedImages.Any()
                ? $"{imageList.Count} saved. {failedImages.Count} failed."
                : "All images saved successfully."
        });

    }


    /// <summary>
    /// Set Cover Image for a property by providing the property's public identifier and the public identifier of the image to be set as cover
    /// </summary>
    /// <param name="propertyPublicId"></param>
    /// <param name="coverImagePublicId"></param>
    /// <param name="updateBy"></param>
    /// <returns></returns>
    [HttpPost("admin-update-property-cover-image")]
    public async Task<ActionResult<APIResponse>> UpdatePropertyCoverImageAsync([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] Guid coverImagePublicId, [FromQuery, BindRequired] string updateBy)
    {
        if (propertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");
        if (coverImagePublicId == Guid.Empty)
            return _responseService.BadRequestResponse("CoverImageId can not be empty");
        if (string.IsNullOrEmpty(updateBy))
            return _responseService.BadRequestResponse("UpdatedBy can not be empty");

        var result = await _unitOfWork.DapperRepo.UpdatePropertyCoverImageAsync(propertyPublicId, coverImagePublicId, updateBy);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);
    }


    /// <summary>
    /// Updates the popularity status of a property identified by its public identifier.
    /// </summary>
    /// <remarks>If <paramref name="propertyPublicId"/> is an empty GUID, the request is rejected with
    /// a bad request response. If the update operation fails, an internal server error response is
    /// returned.</remarks>
    /// <param name="propertyPublicId">The unique identifier of the property to update. Must not be an empty GUID.</param>
    /// <param name="IsPopular">A value indicating whether the property should be marked as popular. Specify <see langword="true"/> to mark
    /// as popular; otherwise, <see langword="false"/>.</param>
    /// <returns>An <see cref="ActionResult{APIResponse}"/> that contains the result of the update operation, indicating
    /// success or failure.</returns>
    [HttpPost("admin-side-update-property-is-popular-or-not")]
    public async Task<ActionResult<APIResponse>> UpdatePropertyIsPopular([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] bool IsPopular)
    {
        if (propertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.AdminPanelUpdatePropertyIsPopularAsync(propertyPublicId, IsPopular);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);
    }

    /// <summary>
    /// Updates the featured status of a property identified by its public ID.
    /// </summary>
    /// <remarks>Returns a bad request response if <paramref name="propertyPublicId"/> is empty.
    /// Returns an internal server error response if the update operation fails.</remarks>
    /// <param name="propertyPublicId">The unique identifier of the property to update. Cannot be an empty GUID.</param>
    /// <param name="isFeatured">A value indicating whether the property should be marked as featured. Specify <see langword="true"/> to mark
    /// as featured; otherwise, <see langword="false"/>.</param>
    /// <returns>An <see cref="ActionResult{APIResponse}"/> containing the result of the update operation, including success
    /// or error information.</returns>
    [HttpPost("admin-side-update-property-is-featured-or-not")]
    public async Task<ActionResult<APIResponse>> UpdatePropertyIsFeatured([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] bool isFeatured)
    {
        if (propertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.AdminPanelUpdatePropertyIsFeaturedAsync(propertyPublicId, isFeatured);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);
    }


    /// <summary>
    /// Inserts a new property with full details supplied in the request DTO.
    /// </summary>
    /// <param name="req">The DTO containing all property data to insert. Must not be null.</param>
    /// <returns>An ActionResult containing an <see cref="APIResponse"/> that indicates the outcome of the insert operation.</returns>
    [HttpPost("admin-insert-full-property")]
    public async Task<ActionResult<APIResponse>> InsertFullPropertyAsync([FromBody] AdminInsertFullPropertyDTO req)
    {
        if (req.PropertyImages.Count > 5)
            return _responseService.BadRequestResponse("Max 5 images per upload.");

        var PropertyPublicId = Guid.NewGuid();

        var result = await _unitOfWork.DapperRepo.InsertFullPropertyAsync(req, PropertyPublicId);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        var imageList = new List<PropertyImageDbDTO>();

        var latestImagesDisplayOrder = await _unitOfWork.DapperRepo.GetLatestPropertyImageDisplayOrderAsync(PropertyPublicId);

        var ImageIndex = latestImagesDisplayOrder.LatestDisplayOrder + 1;
        var failedImages = new List<string>();
        var savedPaths = new List<string>();

        for (int i = 0; i < req.PropertyImages.Count; i++)
        {
            var img = req.PropertyImages[i];

            try
            {
                var base64Data = img.ProfileImageBase64.Contains(',') ? img.ProfileImageBase64.Split(',')[1] : img.ProfileImageBase64;

                var bytes = Convert.FromBase64String(base64Data);
                var extension = ImageHelper.DetectImageExtension(bytes) ?? throw new InvalidOperationException("Unsupported format. Only JPEG, JPG, PNG, WebP allowed.");

                var fileName = $"{PropertyPublicId}_PropImage_{ImageIndex}";

                var uploadResult = await _fileService.SaveBase64ImageAsync(new Base64ImageSaveRequestDTO
                {
                    Base64Image = img.ProfileImageBase64,
                    Extension = extension,
                    FileName = fileName,
                    ImageFolderPath = "UploadFiles/PropertyImage",
                    ThumbnailFolderPath = "ThumbnailImages/TNPropertyImage"
                });

                savedPaths.Add(uploadResult.LocalPath);

                imageList.Add(new PropertyImageDbDTO
                {
                    PropertyImageName = uploadResult.FileName,
                    PropertyImagePath = uploadResult.WebPath,
                    PropertyImageLocalPath = uploadResult.LocalPath,
                    ThumbPropertyImageName = uploadResult.ThumbnailImageName,
                    ThumbPropertyImagePath = uploadResult.ThumbnailWebPath,
                    IsCover = ImageIndex == 1,
                    DisplayOrder = ImageIndex
                });
                ImageIndex++;
            }
            catch (Exception ex)
            {
                failedImages.Add($"Image[{i + 1}]: {ex.Message}");
            }
        }

        if (!imageList.Any())
        {
            await _logService.LogCustomAsync(
                $"[AddPropertyImages] ALL FAILED — PropertyId={PropertyPublicId} | {string.Join(" | ", failedImages)}",
                "ERROR");

            return _responseService.InternalServerResponse($"All images failed. Errors: {string.Join(" | ", failedImages)}");
        }

        var jsonImages = JsonConvert.SerializeObject(imageList);
        var imagesaveres = await _unitOfWork.DapperRepo.AddNewPropertyImagesAsync(PropertyPublicId, req.CreatedBy, jsonImages);

        if (imagesaveres.Result <= 0)
        {
            await _logService.LogCustomAsync($"Image Upload FAILED: {imagesaveres.Message}", "ERROR");
            return _responseService.SuccessResponse("Property saved successfully but image not uploaded");
        }

        return _responseService.SuccessResponse(result.Message);
    }


    /// <summary>
    /// Deletes a property identified by its public identifier.
    /// </summary>
    /// <param name="propertyPublicId">The public GUID of the property to delete. Must not be an empty GUID.</param>
    /// <param name="Modifiedby">Identifier of the user performing the deletion.</param>
    /// <returns>An ActionResult containing an <see cref="APIResponse"/> indicating success or failure.</returns>
    /// <response code="200">Property deleted successfully</response>
    /// <response code="400">Invalid request (e.g. empty PropertyPublicId)</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("admin-delete-prop")]
    public async Task<ActionResult<APIResponse>> DeleteProperty([FromQuery, BindRequired] Guid propertyPublicId, [FromQuery, BindRequired] string Modifiedby)
    {
        if (propertyPublicId == Guid.Empty)
            return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

        var result = await _unitOfWork.DapperRepo.DeletePropertyAsync(propertyPublicId, Modifiedby);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        return _responseService.SuccessResponse(result.Message);
    }

	/// <summary>
	/// Retrieves a list of all property owners for administrative purposes.
	/// </summary>
	/// <returns></returns>
	[HttpGet("get-properties-owner-list")]
	public async Task<ActionResult<APIResponse>> GetPropertiesOwnerList()
	{
		var responce = await _unitOfWork.DapperRepo.AdminPanelOwnerListAsync();
		return _responseService.SuccessResponse(responce);
	}

	/// <summary>
	/// Retrieves detailed view information for a specific property identified by its public identifier.
	/// </summary>
	/// <param name="PropertyPublicId"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	[HttpGet("get-property-view-details")]
	public async Task<ActionResult<APIResponse>> GetPropertyViewDetails([FromQuery, BindRequired] Guid PropertyPublicId, CancellationToken ct)
	{
		if (PropertyPublicId == Guid.Empty)
			return _responseService.BadRequestResponse("PropertyPublicId can not be empty");

		var result = await _unitOfWork.DapperRepo.GetPropertyViewDetailsAsync(PropertyPublicId, ct);

        if (result is null)
            return _responseService.InternalServerResponse("No data found!");

		return _responseService.SuccessResponse(result ?? new());

	}

	#endregion
}
