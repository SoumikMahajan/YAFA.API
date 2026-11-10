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
    /// Administrative endpoints for managing promotional offers.
    /// Covers full CRUD, image uploads, active/inactive toggling,
    /// and paginated listing.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOffersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;
        private readonly IFileRepo _fileService;
        private readonly ILogService _logService;

        /// <summary>
        /// Initializes a new instance of the AdminOffersController class with the specified dependencies.
        /// </summary>
        public AdminOffersController(IUnitOfWork unitOfWork,ApiResponseService responseService,IFileRepo fileService,ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _responseService = responseService;
            _fileService = fileService;
            _logService = logService;
        }

        /// <summary>
        /// Creates a new promotional offer.
        /// Pass <c>OfferImageBase64</c> to attach a banner image;
        /// terms are an ordered string list.
        /// </summary>
        [HttpPost("add-offer")]
        public async Task<ActionResult<APIResponse>> AddOffer([FromBody] AdminAddOfferRequestDto request)
        {
            var offerPublicId = Guid.NewGuid();
            var uploadResult = new ImageUploadResultDTO();

            // Save image if provided
            if (!string.IsNullOrWhiteSpace(request.OfferImageBase64))
            {
                uploadResult = await _fileService.SaveBase64ImageAsync(new Base64ImageSaveRequestDTO
                {
                    Base64Image = request.OfferImageBase64,
                    Extension = request.OfferImageExtension,
                    FileName = $"{offerPublicId}_OfferImage",
                    ImageFolderPath = "UploadFiles/OfferImage",
                    ThumbnailFolderPath = "ThumbnailImages/TNOfferImage"
                });

                if (uploadResult == null)
                    return _responseService.InternalServerResponse("Image upload failed. Please try again.");
            }

            var (result, message) = await _unitOfWork.DapperRepo.AdminAddOfferAsync(request, uploadResult, offerPublicId);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"[AdminOffers] ADD FAILED: {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync(
                $"[AdminOffers] ADD SUCCESS: OfferPublicId={offerPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }


        /// <summary>
        /// Updates an existing offer.
        /// Only non-null properties are applied (true partial update).
        /// Omit <c>Terms</c> to leave existing terms unchanged;
        /// pass an empty list to remove all terms.
        /// </summary>
        [HttpPost("update-offer")]
        public async Task<ActionResult<APIResponse>> UpdateOffer([FromBody] AdminUpdateOfferRequestDto request)
        {
            if (request.OfferPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid OfferPublicId is required.");

            var uploadResult = new ImageUploadResultDTO();

            // Save new banner image only when caller sends one
            if (!string.IsNullOrWhiteSpace(request.OfferImageBase64))
            {
                uploadResult = await _fileService.SaveBase64ImageAsync(new Base64ImageSaveRequestDTO
                {
                    Base64Image = request.OfferImageBase64,
                    Extension = request.OfferImageExtension,
                    FileName = $"{request.OfferPublicId}_OfferImage",
                    ImageFolderPath = "UploadFiles/OfferImage",
                    ThumbnailFolderPath = "ThumbnailImages/TNOfferImage"
                });

                if (uploadResult == null)
                    return _responseService.InternalServerResponse("Image upload failed. Please try again.");
            }

            var (result, message) = await _unitOfWork.DapperRepo.AdminUpdateOfferAsync(request, uploadResult);

            if (result <= 0)
            {
                await _logService.LogCustomAsync(
                    $"[AdminOffers] UPDATE FAILED: OfferPublicId={request.OfferPublicId} | {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync(
                $"[AdminOffers] UPDATE SUCCESS: OfferPublicId={request.OfferPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>Soft-deletes an offer (sets IsDeleted = 1).</summary>
        [HttpPost("delete-offer")]
        public async Task<ActionResult<APIResponse>> DeleteOffer([FromQuery, BindRequired] Guid offerPublicId,[FromQuery, BindRequired] string deletedBy)
        {
            if (offerPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid OfferPublicId is required.");

            if (string.IsNullOrWhiteSpace(deletedBy))
                return _responseService.BadRequestResponse("DeletedBy is required.");

            var (result, message) = await _unitOfWork.DapperRepo.AdminDeleteOfferAsync(offerPublicId, deletedBy);

            if (result <= 0)
            {
                await _logService.LogCustomAsync(
                    $"[AdminOffers] DELETE FAILED: OfferPublicId={offerPublicId} | {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync(
                $"[AdminOffers] DELETE SUCCESS: OfferPublicId={offerPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>Flips the IsActive flag (active ↔ inactive).</summary>
        [HttpPost("toggle-active")]
        public async Task<ActionResult<APIResponse>> ToggleActive([FromQuery, BindRequired] Guid offerPublicId,[FromQuery, BindRequired] string updatedBy)
        {
            if (offerPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid OfferPublicId is required.");

            if (string.IsNullOrWhiteSpace(updatedBy))
                return _responseService.BadRequestResponse("UpdatedBy is required.");

            var (result, message) =
                await _unitOfWork.DapperRepo.AdminToggleOfferActiveAsync(offerPublicId, updatedBy);

            if (result <= 0)
            {
                await _logService.LogCustomAsync(
                    $"[AdminOffers] TOGGLE FAILED: OfferPublicId={offerPublicId} | {message}", "ERROR");
                return _responseService.InternalServerResponse(message);
            }

            await _logService.LogCustomAsync(
                $"[AdminOffers] TOGGLE SUCCESS: OfferPublicId={offerPublicId}", "SUCCESS");

            return _responseService.SuccessResponse(message);
        }

        /// <summary>
        /// Returns a paginated list of all offers (including inactive).
        /// Terms are pre-split into a string array per offer.
        /// </summary>
        [HttpGet("get-all-offers")]
        public async Task<ActionResult<APIResponse>> GetAllOffers([FromQuery, BindRequired] int pageNumber = 1,[FromQuery, BindRequired] int pageSize = 10)
        {

            var result = await _unitOfWork.DapperRepo.AdminGetAllOffersAsync(pageNumber, pageSize);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            if (!result.Item3.Any())
                return _responseService.SuccessResponse("No offers found.");

            return _responseService.SuccessResponse(result.Item3);
        }

        /// <summary>Returns full detail of a single offer including all terms.</summary>
        [HttpGet("get-offer-by-id")]
        public async Task<ActionResult<APIResponse>> GetOfferById([FromQuery, BindRequired] Guid offerPublicId)
        {
            if (offerPublicId == Guid.Empty)
                return _responseService.BadRequestResponse("Valid OfferPublicId is required.");

            var result = await _unitOfWork.DapperRepo.AdminGetOfferByIdAsync(offerPublicId);

            if (result.Result <= 0)
                return _responseService.InternalServerResponse(result.Message);

            if (result.Data == null)
                return _responseService.NotFoundResponse("Offer not found.");

            return _responseService.SuccessResponse(result.Data);
        }
    }
}
