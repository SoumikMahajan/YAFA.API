using System.Diagnostics;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos.User;

namespace Yafa.Infrastructure.Job;

public class CheckInReminderJob
{
	private const string JOB_NAME = "CheckInReminderJob";
	private readonly IUnitOfWork _unitOfWork;
	private readonly IFirebaseService _firebase;
	private readonly ILogService _logService;

	public CheckInReminderJob(IUnitOfWork unitOfWork,IFirebaseService firebase,ILogService logService)
	{
		_unitOfWork = unitOfWork;
		_firebase = firebase;
		_logService = logService;
	}

	public async Task SendCheckInRemindersAsync()
	{
		var jobStart = DateTime.UtcNow;
		var stopwatch = Stopwatch.StartNew();

		// ── JOB STARTED ──────────────────────────────────────────────
		await _logService.LogJobAsync(JOB_NAME, "STARTED",$"Job triggered at {jobStart:HH:mm:ss} UTC");

		IEnumerable<CheckInReminderDto> bookings;

		try
		{
			bookings = await _unitOfWork.DapperRepo.GetTomorrowCheckInsAsync();
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			await _logService.LogExceptionErrorAsync(ex);
			await _logService.LogJobAsync(JOB_NAME, "FAILED",$"Failed to fetch bookings from DB. Error: {ex.Message}",stopwatch.Elapsed);
			return;
		}

		var bookingList = bookings.ToList();
		if (!bookingList.Any())
		{
			stopwatch.Stop();
			await _logService.LogJobAsync(JOB_NAME, "COMPLETED","No check-ins scheduled for tomorrow. Nothing to process.",stopwatch.Elapsed);
			return;
		}

		await _logService.LogJobAsync(JOB_NAME, "PROCESSING",$"Found {bookingList.Count} booking(s) for tomorrow. Starting notifications.");

		int successCount = 0;
		int failedCount = 0;
		var failedBookings = new List<string>();

		foreach (var booking in bookingList)
		{
			try
			{
				await ProcessSingleReminderAsync(booking);
				successCount++;
			}
			catch (Exception ex)
			{
				failedCount++;
				failedBookings.Add(booking.BookingPublicId.ToString());

				await _logService.LogExceptionErrorAsync(ex);
				//await _logService.LogJobAsync(JOB_NAME, "WARNING",$"Failed for BookingId: {booking.BookingPublicId} | Error: {ex.Message} | Continuing...");
			}
		}

		stopwatch.Stop();

		var summary = $"Total: {bookingList.Count} | Success: {successCount} | Failed: {failedCount}";

		if (failedCount > 0)
			summary += $" | FailedBookings: [{string.Join(", ", failedBookings)}]";

		await _logService.LogJobAsync(JOB_NAME,failedCount == 0 ? "COMPLETED" : "COMPLETED_WITH_ERRORS",summary,stopwatch.Elapsed);
	}
	private async Task ProcessSingleReminderAsync(CheckInReminderDto booking)
	{
		var bookingRef = booking.BookingPublicId.ToString();

		// 1. In-app notification
		await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
			booking.UserId,
			"Check-In Tomorrow",
			$"Your stay at {booking.PropertyName} starts tomorrow ({booking.CheckInDate:dd MMM yyyy}). Get ready!",
			NotificationType.CheckInReminder,
			bookingRef
		);

		var checkInTimeFormatted = DateTime.Today.Add(booking.Checkintime).ToString("hh:mm tt");

		// 2. Push notification
		var deviceToken = await _unitOfWork.DapperRepo
			.GetUserActiveDeviceTokenAsync(booking.UserId);

		if (deviceToken.Any())
		{
			await _firebase.SendBulkAsync(
				deviceToken,
				"Check-In Tomorrow! 🏡",
				$"Your stay at {booking.PropertyName} starts tomorrow. Check-in: {checkInTimeFormatted}",
				new Dictionary<string, string>
				{
					{ "type",        NotificationType.CheckInReminder },
					{ "referenceId", bookingRef }
				}
			);
		}
	}


}
