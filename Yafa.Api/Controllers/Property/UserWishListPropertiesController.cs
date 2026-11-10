using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Property;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Controllers.Property
{
    /// <summary>
    /// Provides API endpoints for managing and retrieving properties in a user's wishlist.
    /// </summary>
    /// <remarks>All actions require authentication. This controller enables clients to retrieve the list of
    /// properties in a user's wishlist and to add or remove properties from the wishlist. Responses indicate the
    /// outcome of each operation, including success, not found, or invalid request scenarios.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserWishListPropertiesController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;

        /// <summary>
        /// Initializes a new instance of the UserWishListPropertiesController class with the specified logging, unit of
        /// work, configuration, and response services.
        /// </summary>
        /// <param name="logService">The service used for logging application events and errors.</param>
        /// <param name="unitOfWork">The unit of work instance that manages data access and transactions.</param>
        /// <param name="configuration">The application configuration settings.</param>
        /// <param name="responseService">The service used to generate standardized API responses.</param>
        public UserWishListPropertiesController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
        }

        /// <summary>
        /// Handles a request to retrieve the list of properties in a user's wishlist.
        /// </summary>
        /// <remarks>Returns a 404 response if the user's wishlist contains no properties. Returns a 400
        /// response if the request data is invalid.</remarks>
        /// <param name="request">An object containing the criteria for identifying the user's wishlist. Cannot be null.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An ActionResult containing an APIResponse with the list of properties in the user's wishlist if found;
        /// otherwise, a response indicating that no properties were found or the request was invalid.</returns>
        [HttpPost("get-user-wishlist-properties")]
        public async Task<ActionResult<APIResponse>> GetUserWishListProperties([FromBody] WishListRequestDTO request, CancellationToken cancellationToken)
        {
            if (request == null)
                return _responseService.BadRequestResponse("Invalid request data.");

            var result = await _unitOfWork.DapperRepo.GetUserWishListPropertiesAsync(request, cancellationToken);

            if (result == null || !result.Any())
                return _responseService.NotFoundResponse("No properties found in the user's wishlist.");

            return _responseService.SuccessResponse(result);
        }

        /// <summary>
        /// Adds or removes a property from the user's wishlist based on the specified request data.
        /// </summary>
        /// <param name="request">An object containing the details required to add or remove a property from the wishlist. Cannot be null.</param>
        /// <returns>An API response indicating the result of the operation. Returns a success response if the property was added
        /// or removed; otherwise, returns a not found or bad request response.</returns>
        [HttpPost("add-remove-wishlist-property")]
        public async Task<ActionResult<APIResponse>> AddRemoveWishListProperty([FromBody] AddRemoveWishListPropertyDTO request)
        {
            if (request == null)
                return _responseService.BadRequestResponse("Invalid request data.");

            var (result, message) = await _unitOfWork.DapperRepo.AddRemoveWishListPropertyAsync(request);

            if (result <= 0)
            {
                await _logService.LogCustomAsync($"AddRemoveWishListProperty FAILED: {message}", "ERROR");
                return _responseService.NotFoundResponse(message);
            }

            await _logService.LogCustomAsync($"AddRemoveWishListProperty SUCCESS: PropertyId={request.PropertyPublicId}", "SUCCESS");
            return _responseService.SuccessResponse(message);

        }
    }
}
