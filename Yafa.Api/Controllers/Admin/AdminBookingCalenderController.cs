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
/// Controller for managing admin booking calendar operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminBookingCalenderController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiResponseService _responseService;
    private readonly IFileRepo _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminBookingCalenderController"/> class.
    /// </summary>
    public AdminBookingCalenderController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService)
    {
        _logService = logService;
        _unitOfWork = unitOfWork;
        _responseService = responseService;
        _fileService = fileService;
    }

    #region :: Properties

    /// <summary>
    /// Gets all bookings on the calendar for a specified date range and property.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("get-all-bookings-on-calender")]
    public async Task<ActionResult<APIResponse>> GetBookingsOnCalender([FromQuery, BindRequired] CalendarBookingRequestDTO request)
    {
        if (string.IsNullOrEmpty(request.startDate) || string.IsNullOrEmpty(request.endDate))
        {
            return _responseService.BadRequestResponse("Start date and end date are required");
        }

        var result = await _unitOfWork.DapperRepo.GetBookingsOnCalenderAsync(request);

        if(result.Result <= 0)
            return _responseService.NotFoundResponse("Something went wrong while fetching bookings");

        if(result.Item3 == null || !result.Item3.Any())
            return _responseService.NotFoundResponse("No bookings found for the given date range");

        return _responseService.SuccessResponse(result.Item3);

    }




    #endregion
}
