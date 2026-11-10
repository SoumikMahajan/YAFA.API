using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;
using Yafa.Infrastructure.Job;

namespace Yafa.Api.Controllers.User
{
	/// <summary>
	/// Represents an API controller that manages notification-related operations for authenticated users.
	/// </summary>
	/// <remarks>All actions in this controller require authentication. The controller is configured with
	/// dependency injection for logging, unit of work, JWT handling, file repository access, and standardized API
	/// responses. The route for this controller is prefixed with 'api/notification'.</remarks>
	[Route("api/[controller]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly ILogService _logService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApiResponseService _responseService;
		private readonly IFileRepo _fileService;
		private readonly IFirebaseService _firebase;

		/// <summary>
		/// Initializes a new instance of the NotificationController class with the specified services and configuration.
		/// </summary>
		/// <param name="logService">The logging service used to record application events and errors.</param>
		/// <param name="unitOfWork">The unit of work instance used to manage database transactions.</param>
		/// <param name="configuration">The application configuration settings.</param>
		/// <param name="responseService">The service used to generate standardized API responses.</param>
		/// <param name="fileService">The file repository service used for file operations.</param>
		/// <param name="firebase">The Firebase service used for sending notifications.</param>
		public NotificationController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService, IFirebaseService firebase)
		{
			_logService = logService;
			_unitOfWork = unitOfWork;
			_responseService = responseService;
			_fileService = fileService;
			_firebase = firebase;
		}

		/// <summary>
		/// Broadcasts a notification to all active device tokens using the specified request data.
		/// </summary>
		/// <remarks>This method sends notifications in batches to all active device tokens and deactivates
		/// permanently invalid tokens. It also logs the broadcast and inserts in-app notifications. The operation is
		/// asynchronous.</remarks>
		/// <param name="request">The notification details to broadcast, including the title and body. The title and body must not be null or
		/// whitespace.</param>
		/// <returns>An API response containing the result of the broadcast operation, including counts of successful and failed
		/// notifications. Returns a bad request response if the title or body is missing, a not found response if no active
		/// device tokens are available, or an internal server error response if logging the broadcast fails.</returns>
		[Authorize(Roles = "Admin")]
		[HttpPost("broadcast-notification")]
		public async Task<ActionResult<APIResponse>> BroadcastNotification([FromBody] BroadcastNotificationRequestDTO request)
		{
			if(string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body) || string.IsNullOrWhiteSpace(request.ReferencePublicId))
			{
				return _responseService.BadRequestResponse("Title, Body, and ReferencePublicId are required for broadcasting notifications.");
			}

			var data = new Dictionary<string, string>
			{
				{ "type", NotificationType.Offer },
				{ "offerId", request.ReferencePublicId.ToString() },
				{ "action", "OPEN_SCREEN" }
			};

			// 1. Fetch all active tokens from DB
			var devices = (await _unitOfWork.DapperRepo.GetActiveDeviceTokensAsync()).ToList();

			//var devices = new List<ActiveDeviceTokenDTO>
			//{
			//	new ActiveDeviceTokenDTO
			//	{
			//		UserId = Guid.Parse("BFECBE97-4DF1-496B-A6F6-B86F5D89C776"),
			//		Platform = "android",
			//		FirebaseToken = "d3Z5Qh0_TCms7imcw4ZJMl:APA91bFngu-L6IGOeKhTLDKYT300nizHjjRPeqQDczm2KucA2U1VqjmM9MG_-_2DOhguq6P2wN8xEybPIaYqyBqEBQUOqfr3Lj_sCDBckUdN4fVwzlCSOQM"
			//	}
			//};



			if (!devices.Any())
				return _responseService.NotFoundResponse("No active device tokens found.");

			var tokens = devices.Select(d => d.FirebaseToken).ToList();

			// 2. Send in batches — invalid tokens collected automatically
			var (successCount, failedTokens) = await _firebase.SendBulkAsync(tokens,request.Title,request.Body, data);

			// 3. Deactivate permanently invalid tokens in DB
			int deactivatedCount = 0;
			if (failedTokens.Any())
			{
				deactivatedCount = await _unitOfWork.DapperRepo.DeactivateInvalidTokensAsync(failedTokens);
			}

			// 4. Insert in-app notifications + log the broadcast
			var responce = await _unitOfWork.DapperRepo.InsertBulkNotificationAsync(request, successCount,failedTokens.Count);

			if(responce.Result <= 0)
			{
				return _responseService.InternalServerResponse("Failed to log broadcast notification.");
			}

			var result = new BroadcastNotificationResultDTO
			{
				TotalTokens = tokens.Count,
				SuccessCount = successCount,
				FailureCount = failedTokens.Count,
				InvalidTokensDeactivated = deactivatedCount,
				BatchesProcessed = _firebase.GetBatchCount(tokens.Count)
			};

			return _responseService.SuccessResponse(result);
		}

		/// <summary>
		/// Notification
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpPost("send-notification")]
		public async Task<ActionResult<APIResponse>> Send(NotificationRequestDTO request)
		{
			var data = new Dictionary<string, string>
			{
				{ "type", "offer" },
				{ "offerId", "789" },
				{ "action", "OPEN_SCREEN" }
			};

			var result = await _firebase.SendAsync(
				request.DeviceToken,
				request.Title,
				request.Body,
				data
			);

			return _responseService.SuccessResponse(new
			{
				Message = result
			});
		}

		/// <summary>
		/// Retrieves the count of unread notifications for the specified user.
		/// </summary>
		/// <param name="userId">The unique identifier of the user for whom to retrieve the unread notification count. Cannot be null or empty.</param>
		/// <returns>An ActionResult containing an APIResponse with the unread notification count for the specified user.</returns>
		[Authorize]
		[HttpGet("unread-count-notification-from-userend")]
		public async Task<ActionResult<APIResponse>> GetUnreadCountNotification([FromQuery, BindRequired] string userId)
		{
			if(string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId is required to fetch unread notification count.");

			var count = await _unitOfWork.DapperRepo.GetUnreadNotificationCountAsync(userId);

			return _responseService.SuccessResponse(new { UnreadCount = count });
		}

		/// <summary>
		/// Retrieves all notifications associated with the specified user identifier.
		/// </summary>
		/// <param name="userId">The unique identifier of the user whose notifications are to be retrieved. Cannot be null or empty.</param>
		/// <returns>An API response containing a list of notifications for the specified user.</returns>
		[Authorize]
		[HttpGet("get-all-notifications-from-userend")]
		public async Task<ActionResult<APIResponse>> GetAllNotifications([FromQuery, BindRequired] string userId)
		{
			if(string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId is required to fetch notifications.");

			var list = await _unitOfWork.DapperRepo.GetAllNotificationsAsync(userId);

			return _responseService.SuccessResponse(list);
		}

		/// <summary>
		/// Marks the specified notification as read for the given user.
		/// </summary>
		/// <param name="notificationId">The unique identifier of the notification to mark as read. Must be greater than zero.</param>
		/// <param name="userId">The identifier of the user for whom the notification will be marked as read. Cannot be null or empty.</param>
		/// <returns>An ActionResult containing an APIResponse that indicates the result of the operation. Returns a success response
		/// if the notification is marked as read; otherwise, returns a bad request response with an error message.</returns>
		[Authorize]
		[HttpPost("mark-read-notification-from-userend")]
		public async Task<ActionResult<APIResponse>> MarkReadNotification([FromQuery, BindRequired] long notificationId, [FromQuery, BindRequired] string userId)
		{
			if(string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId is required to mark notification as read.");

			if(notificationId <= 0)
				return _responseService.BadRequestResponse("Valid NotificationId is required to mark notification as read.");

			var (result, message) = await _unitOfWork.DapperRepo.MarkNotificationReadAsync(notificationId, userId);

			return _responseService.SuccessResponse(new { Message = message });
		}

		/// <summary>
		/// Marks all notifications as read for the specified user.
		/// </summary>
		/// <param name="userId">The unique identifier of the user whose notifications will be marked as read. Cannot be null or empty.</param>
		/// <returns>An API response containing the number of notifications updated and a status message.</returns>
		[Authorize]
		[HttpPost("read-all-notifications-from-userend")]
		public async Task<ActionResult<APIResponse>> MarkAllReadNotifications([FromQuery, BindRequired] string userId)
		{
			if(string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId is required to mark all notifications as read.");

			var (count, message) = await _unitOfWork.DapperRepo.MarkAllNotificationsReadAsync(userId);

			return _responseService.SuccessResponse(new { Updated = count, Message = message });
		}

		/// <summary>
		/// Deletes a notification for a specified user.
		/// </summary>
		/// <param name="notificationId">The unique identifier of the notification to delete. Must be greater than zero.</param>
		/// <param name="userId">The identifier of the user associated with the notification. Cannot be null or empty.</param>
		/// <returns>An API response indicating the result of the delete operation. Returns a success response with a message if the
		/// notification is deleted; otherwise, returns a bad request response if the input parameters are invalid.</returns>
		[Authorize]
		[HttpPost("delete-notification-from-userend")]
		public async Task<ActionResult<APIResponse>> DeleteNotification([FromQuery, BindRequired] long notificationId, [FromQuery, BindRequired] string userId)
		{
			if(string.IsNullOrEmpty(userId))
				return _responseService.BadRequestResponse("UserId is required to delete notification.");

			if(notificationId <= 0)
				return _responseService.BadRequestResponse("Valid NotificationId is required to delete notification.");

			var (result, message) = await _unitOfWork.DapperRepo.DeleteNotificationAsync(notificationId, userId);

			return _responseService.SuccessResponse(new { Message = message });
		}

		//[Authorize]
		//[HttpPost("trigger-checkin-reminder")]
		//public ActionResult TriggerNow()
		//{
		//	// Enqueue immediately — runs in seconds
		//	BackgroundJob.Enqueue<CheckInReminderJob>(
		//		job => job.SendCheckInRemindersAsync()
		//	);

		//	return Ok(new { Message = "Job enqueued. Check /hangfire dashboard." });
		//}

	}
}
