using System.Diagnostics;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos.User;

namespace Yafa.Infrastructure.Job
{
	public class CheckOutReminderJob
	{
		private const string JOB_NAME = "CheckOutReminderJob";
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFirebaseService _firebase;
		private readonly ILogService _logService;

		public CheckOutReminderJob(IUnitOfWork unitOfWork,IFirebaseService firebase,ILogService logService)
		{
			_unitOfWork = unitOfWork;
			_firebase = firebase;
			_logService = logService;
		}

		public async Task SendCheckOutRemindersAsync()
		{
			var jobStart = DateTime.UtcNow;
			var stopwatch = Stopwatch.StartNew();

			// ── JOB STARTED ──────────────────────────────────────────────
			await _logService.LogJobAsync(JOB_NAME, "STARTED", $"Job triggered at {jobStart:HH:mm:ss} UTC");

			IEnumerable<CheckInReminderDto> bookings;

			try
			{
				bookings = await _unitOfWork.DapperRepo.GetTodayCheckOutsAsync();
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				await _logService.LogExceptionErrorAsync(ex);
				await _logService.LogJobAsync(JOB_NAME, "FAILED", $"Failed to fetch bookings from DB. Error: {ex.Message}", stopwatch.Elapsed);
				return;
			}

			var bookingList = bookings.ToList();
			if (!bookingList.Any())
			{ 
				stopwatch.Stop();
				await _logService.LogJobAsync(JOB_NAME, "COMPLETED", "No check-outs scheduled for today. Nothing to process.", stopwatch.Elapsed);
				return;
			}

			await _logService.LogJobAsync(JOB_NAME, "PROCESSING", $"Processing {bookingList.Count} check-out reminders.", stopwatch.Elapsed);

			int successCount = 0;
			int failedCount = 0;
			var failedBookings = new List<string>();

			foreach (var booking in bookingList)
			{
				try
				{
					await ProcessSingleCheckOutAsync(booking);
					successCount++;
				}
				catch (Exception ex)
				{
					failedCount++;
					failedBookings.Add(booking.BookingPublicId.ToString());

					await _logService.LogExceptionErrorAsync(ex);
					//await _logService.LogJobAsync(JOB_NAME, "ERROR", $"Failed to process check-out reminder for Booking ID: {booking.BookingPublicId}. Error: {ex.Message}", stopwatch.Elapsed);
				}
			}

			stopwatch.Stop();
			
			var summary = $"Total: {bookingList.Count} | Success: {successCount} | Failed: {failedCount}";

			if (failedCount > 0)
				summary += $" | FailedBookings: [{string.Join(", ", failedBookings)}]";

			await _logService.LogJobAsync(JOB_NAME, failedCount == 0 ? "COMPLETED" : "COMPLETED_WITH_ERRORS", summary, stopwatch.Elapsed);
		}

		private async Task ProcessSingleCheckOutAsync(CheckInReminderDto booking)
		{
			var bookingRef = booking.BookingPublicId.ToString();

			var checkOutTimeFormatted = DateTime.Today.Add(booking.Checkouttime).ToString("hh:mm tt");

			await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
				booking.UserId,
				"Check-Out Today",
				$"Your stay at {booking.PropertyName} ends today ({booking.CheckOutDate:dd MMM yyyy}). Check-out by {checkOutTimeFormatted}.",
				NotificationType.CheckOutReminder,
				bookingRef
			);

			var deviceToken = await _unitOfWork.DapperRepo
				.GetUserActiveDeviceTokenAsync(booking.UserId);

			if (deviceToken.Any())
			{
				await _firebase.SendBulkAsync(
					deviceToken,
					"Check-Out Today 🧳",
					$"Your stay at {booking.PropertyName} ends today. Check-out by {checkOutTimeFormatted}.",
					new Dictionary<string, string>
					{
						{ "type",        NotificationType.CheckOutReminder },
						{ "referenceId", bookingRef }
					}
				);
			}
		}
	}
}
