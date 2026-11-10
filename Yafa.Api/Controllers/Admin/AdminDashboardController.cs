using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Api.Controllers.Admin
{
	/// <summary>
	/// Controller for admin dashboard related operations.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class AdminDashboardController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApiResponseService _responseService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminDashboardController"/> class.
		/// </summary>
		/// <param name="unitOfWork"></param>
		/// <param name="responseService"></param>
		public AdminDashboardController(IUnitOfWork unitOfWork, ApiResponseService responseService)
		{
			_unitOfWork = unitOfWork;
			_responseService = responseService;
		}

		/// <summary>
		/// Get the dashboard summary for the admin panel.
		/// </summary>
		/// <returns></returns>
		[HttpGet("admin-get-dashboard-summary")]
		public async Task<ActionResult<APIResponse>> AdminGetDashboardSummary()
		{
			AdminDashboardDto? response = await _unitOfWork.DapperRepo.AdminGetDashboardSummaryAsync();

			//var bookingListRequest = new AllBookedDetailRequestDTO
			//{
			//	PageNumber = 1,
			//	PageSize = 5
			//};

			//Top 5 bookings for admin dashboard
			//var result = await _unitOfWork.DapperRepo.AdminBookingListItemAsync(bookingListRequest);

			//if (result.Item3.Any())
			//{
			//	response.RecentBookings = result.Item3;
			//}

			return _responseService.SuccessResponse(response);
		}

		/// <summary>Revenue and bookings per month, last 12 months (current month flagged).</summary>
		[HttpGet("admin-get-revenue-trend")]
		public async Task<ActionResult<APIResponse>> AdminGetRevenueTrend()
		{
			var response = await _unitOfWork.DapperRepo.AdminGetRevenueTrendAsync();
			return _responseService.SuccessResponse(response);
		}
	}
}
