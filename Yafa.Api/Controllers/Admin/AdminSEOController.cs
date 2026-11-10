using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Api.Controllers.Admin;

/// <summary>
/// Controller for administrative SEO operations.
/// </summary>
/// <remarks>
/// All endpoints are secured to users in the "Admin" role. This controller exposes
/// actions to manage SEO content entries and SEO page master records using the
/// application's Dapper repository via <see cref="IUnitOfWork.DapperRepo"/>.
/// Responses are normalized using <see cref="ApiResponseService"/>, and failures are
/// recorded through <see cref="ILogService"/>.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminSEOController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiResponseService _responseService;
    private readonly ILogService _logService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminSEOController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work providing repository access (DapperRepo used by this controller).</param>
    /// <param name="responseService">Service that builds standardized API responses.</param>
    /// <param name="logService">Service used to record custom logs on failures.</param>
    public AdminSEOController(IUnitOfWork unitOfWork, ApiResponseService responseService, ILogService logService)
    {
        _unitOfWork = unitOfWork;
        _responseService = responseService;
        _logService = logService;
    }

    /// <summary>
    /// Adds a new SEO content record.
    /// </summary>
    /// <param name="request">The SEO content creation DTO containing required fields.</param>
    /// <returns>
    /// An <see cref="ActionResult{APIResponse}"/> indicating success with a message on success,
    /// or an internal server response on failure. Failure cases are logged via <see cref="_logService"/>.
    /// </returns>
    [HttpPost("add-seo-content")]
    public async Task<ActionResult<APIResponse>> AddSEOContent([FromBody] SeoContentAddRequestDto request)
    {
        var (result, message) = await _unitOfWork.DapperRepo.AddSeoContentAsync(request);

        if (result <= 0)
        {
            await _logService.LogCustomAsync($"[AdminSEO] ADD FAILED: {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Updates an existing SEO content record.
    /// </summary>
    /// <param name="request">The SEO content update DTO. Must include the target <c>SeoContentID</c>.</param>
    /// <returns>
    /// A standardized API response indicating the result. On failure the update is logged and
    /// an internal server response is returned.
    /// </returns>
    [HttpPost("update-seo-content")]
    public async Task<ActionResult<APIResponse>> UpdateSEOContent([FromBody] SeoContentUpdateRequestDto request)
    {
        var (result, message) = await _unitOfWork.DapperRepo.UpdateSeoContentAsync(request);

        if (result <= 0)
        {
            await _logService.LogCustomAsync(
                $"[AdminSEO] UPDATE FAILED: SeoContentID={request.SeoContentID} | {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Deletes an SEO content record.
    /// </summary>
    /// <param name="seoContentId">Identifier of the SEO content to delete.</param>
    /// <param name="deletedBy">Username or identifier of the user performing the deletion. Required.</param>
    /// <returns>
    /// A standardized API response. Returns a bad request response if <c>deletedBy</c> is not provided.
    /// On failure the deletion is logged and an internal server response is returned.
    /// </returns>
    [HttpPost("delete-seo-content")]
    public async Task<ActionResult<APIResponse>> DeleteSEOContent([FromQuery, BindRequired] int seoContentId, [FromQuery, BindRequired] string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            return _responseService.BadRequestResponse("DeletedBy is required.");

        var (result, message) = await _unitOfWork.DapperRepo.DeleteSeoContentAsync(seoContentId, deletedBy);

        if (result <= 0)
        {
            await _logService.LogCustomAsync(
                $"[AdminSEO] DELETE FAILED: SeoContentID ={seoContentId} | {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Retrieves a paginated list of SEO content records.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based). Default is 1.</param>
    /// <param name="pageSize">Number of items per page. Default is 10.</param>
    /// <returns>
    /// A standardized API response containing the list of SEO content records. If no records are found
    /// a success response with a descriptive message is returned. Repository errors return an internal server response.
    /// </returns>
    [HttpGet("get-all-seo-content")]
    public async Task<ActionResult<APIResponse>> GetAllSEOContent([FromQuery, BindRequired] int pageNumber = 1, [FromQuery, BindRequired] int pageSize = 10)
    {
        var result = await _unitOfWork.DapperRepo.GetSeoContentListAsync(pageNumber, pageSize);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        if (!result.Item3.Any())
            return _responseService.SuccessResponse("No Seo content found.");

        return _responseService.SuccessResponse(result.Item3);
    }


    // ---------- SeoPageMaster ----------

    /// <summary>
    /// Adds a new SEO page master entry.
    /// </summary>
    /// <param name="request">The DTO containing values required to create a page master record.</param>
    /// <returns>
    /// A standardized API response indicating success or failure. Failures are logged and returned as internal server responses.
    /// </returns>
    [HttpPost("add-seo-page-master")]
    public async Task<ActionResult<APIResponse>> AddSEOPageMaster([FromBody] SeoPageMasterAddRequestDto request)
    {
        var (result, message) = await _unitOfWork.DapperRepo.AddSeoPageMasterAsync(request);

        if (result <= 0)
        {
            await _logService.LogCustomAsync($"[AdminSEOPageMaster] ADD FAILED: {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Updates an existing SEO page master record.
    /// </summary>
    /// <param name="request">The update DTO. Must include the target <c>PageKeyID</c>.</param>
    /// <returns>
    /// A standardized API response. Failures are logged and returned as internal server responses.
    /// </returns>
    [HttpPost("update-seo-page-master")]
    public async Task<ActionResult<APIResponse>> UpdateSEOPageMaster([FromBody] SeoPageMasterUpdateRequestDto request)
    {
        var (result, message) = await _unitOfWork.DapperRepo.UpdateSeoPageMasterAsync(request);

        if (result <= 0)
        {
            await _logService.LogCustomAsync(
                $"[AdminSEOPageMaster] UPDATE FAILED: PageKeyID={request.PageKeyID} | {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Deletes an SEO page master record.
    /// </summary>
    /// <param name="PageKeyID">Identifier of the page master record to delete.</param>
    /// <param name="deletedBy">Username or identifier of the user performing the deletion. Required.</param>
    /// <returns>
    /// A standardized API response. Returns bad request if <c>deletedBy</c> is not provided. Failures are logged and returned as internal server responses.
    /// </returns>
    [HttpPost("delete-seo-page-master")]
    public async Task<ActionResult<APIResponse>> DeleteSEOPageMaster([FromQuery, BindRequired] int PageKeyID, [FromQuery, BindRequired] string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            return _responseService.BadRequestResponse("DeletedBy is required.");

        var (result, message) = await _unitOfWork.DapperRepo.DeleteSeoPageMasterAsync(PageKeyID, deletedBy);

        if (result <= 0)
        {
            await _logService.LogCustomAsync(
                $"[AdminSEOPageMaster] DELETE FAILED: PageKeyID ={PageKeyID} | {message}", "ERROR");
            return _responseService.InternalServerResponse(message);
        }

        return _responseService.SuccessResponse(message);
    }


    /// <summary>
    /// Retrieves a paginated list of SEO page master records.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based). Default is 1.</param>
    /// <param name="pageSize">Number of items per page. Default is 10.</param>
    /// <returns>
    /// A standardized API response containing the list of page master records or a descriptive success message if none are found.
    /// Repository errors return an internal server response.
    /// </returns>
    [HttpGet("get-all-seo-page-master")]
    public async Task<ActionResult<APIResponse>> GetAllSEOPageMaster([FromQuery, BindRequired] int pageNumber = 1, [FromQuery, BindRequired] int pageSize = 10)
    {
        var result = await _unitOfWork.DapperRepo.GetSeoPageMasterListAsync(pageNumber, pageSize);

        if (result.Result <= 0)
            return _responseService.InternalServerResponse(result.Message);

        if (!result.Item3.Any())
            return _responseService.SuccessResponse("No Seo Page Master found.");

        return _responseService.SuccessResponse(result.Item3);
    }
}
