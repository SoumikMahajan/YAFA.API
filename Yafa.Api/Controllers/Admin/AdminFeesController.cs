using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Controllers.Admin
{
    /// <summary>
    /// Controller for managing fee types in the admin panel. Provides endpoints for retrieving fee types and their details.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminFeesController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;

        /// <summary>
        /// Constructor for AdminFeesController. Initializes the controller with necessary services for logging, data access, and response formatting.
        /// </summary>
        /// <param name="logService"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        /// <param name="responseService"></param>
        public AdminFeesController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
        }

        /// <summary>
        /// Endpoint to retrieve all fee types for the admin panel. This endpoint allows administrators to view all available fee types in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin-get-all-fee-types")]
        public async Task<ActionResult<APIResponse>> AdminGetAllFeeTypes()
        {
            var response = await _unitOfWork.DapperRepo.AdminGetAllFeeTypesAsync();

            return _responseService.SuccessResponse(response);
        }

        /// <summary>
        /// Endpoint to add a new fee type to the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("admin-add-fee-type")]
        public async Task<ActionResult<APIResponse>> AdminAddFeeType([FromBody] AdminAddFeeRequestDto model)
        {
            var response = await _unitOfWork.DapperRepo.AdminAddFeeAsync(model);

            if (response.Result <= 0)
                return _responseService.InternalServerResponse(response.Message);

            return _responseService.SuccessResponse(response.Message);
        }

        /// <summary>
        /// Endpoint to update an existing fee type in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("admin-update-fee-type")]
        public async Task<ActionResult<APIResponse>> AdminUpdateFeeType([FromBody] AdminUpdateFeeTypeDTO model)
        {
            var response = await _unitOfWork.DapperRepo.AdminUpdateFeeAsync(model);
            if (response.Result <= 0)
                return _responseService.InternalServerResponse(response.Message);
            return _responseService.SuccessResponse(response.Message);
        }

        /// <summary>
        /// Endpoint to toggle the active status of a fee type. This allows administrators to enable or disable specific fee types without deleting them from the system.
        /// </summary>
        /// <param name="feePublicId"></param>
        /// <param name="isActive"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        [HttpPost("admin-toggle-fee-type")]
        public async Task<ActionResult<APIResponse>> AdminToggleFeeType([FromQuery, BindRequired] Guid feePublicId, [FromQuery, BindRequired] string updatedBy, [FromQuery, BindRequired] bool isActive = true)
        {
            if(feePublicId == Guid.Empty || string.IsNullOrEmpty(updatedBy))
                return _responseService.BadRequestResponse("Invalid parameters.");

            if(string.IsNullOrEmpty(updatedBy))
                return _responseService.BadRequestResponse("UpdatedBy cannot be null or empty.");

            var response = await _unitOfWork.DapperRepo.AdminToggleFeeTypeAsync(feePublicId, isActive, updatedBy);

            if (response.Result <= 0)
                return _responseService.InternalServerResponse(response.Message);

            return _responseService.SuccessResponse(response.Message);
        }
    }
}
