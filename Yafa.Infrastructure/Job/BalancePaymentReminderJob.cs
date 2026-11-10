using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos.User;

namespace Yafa.Infrastructure.Job
{
	public class BalancePaymentReminderJob
	{
		private const string JOB_NAME = "BalancePaymentReminderJob";
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFirebaseService _firebase;
		private readonly ILogService _logService;

		public BalancePaymentReminderJob(IUnitOfWork unitOfWork, IFirebaseService firebase, ILogService logService)
		{
			_unitOfWork = unitOfWork;
			_firebase = firebase;
			_logService = logService;
		}

		public async Task SendBalancePaymentRemindersAsync()
		{
			//var jobStart = DateTime.UtcNow;
			//var stopwatch = Stopwatch.StartNew();

			//await _logService.LogJobAsync(JOB_NAME, "STARTED", $"Job triggered at {jobStart:HH:mm:ss} UTC");

			IEnumerable<BalancePaymentReminderDto> bookings;

			try
			{
				bookings = await _unitOfWork.DapperRepo.GetPendingBalancePaymentRemindersAsync();
			}
			catch (Exception ex)
			{
				//stopwatch.Stop();
				await _logService.LogExceptionErrorAsync(ex);
				//await _logService.LogJobAsync(JOB_NAME, "FAILED", $"Failed to fetch bookings from DB. Error: {ex.Message}", stopwatch.Elapsed);
				return;
			}

			var bookingList = bookings.ToList();
			if (!bookingList.Any())
			{
				//stopwatch.Stop();
				//await _logService.LogJobAsync(JOB_NAME, "COMPLETED", "No pending balance payments today. Nothing to process.", stopwatch.Elapsed);
				return;
			}

			//await _logService.LogJobAsync(JOB_NAME, "PROCESSING", $"Found {bookingList.Count} booking(s) with an unpaid balance. Starting notifications.");

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
				}
			}

			//stopwatch.Stop();

			var summary = $"Total: {bookingList.Count} | Success: {successCount} | Failed: {failedCount}";

			if (failedCount > 0)
				summary += $" | FailedBookings: [{string.Join(", ", failedBookings)}]";

			//await _logService.LogJobAsync(JOB_NAME, failedCount == 0 ? "COMPLETED" : "COMPLETED_WITH_ERRORS", summary, stopwatch.Elapsed);
		}

		private async Task ProcessSingleReminderAsync(BalancePaymentReminderDto booking)
		{
			var bookingRef = booking.BookingPublicId.ToString();
			var isDueToday = booking.CheckInDate == DateOnly.FromDateTime(DateTime.UtcNow);

			var title = isDueToday ? "Balance Due Today" : "Balance Payment Reminder";
			var body = isDueToday
				? $"Your balance of {booking.RemainingBalance:N0} AED for {booking.PropertyName} is due today — pay now to keep your booking."
				: $"You still owe {booking.RemainingBalance:N0} AED for {booking.PropertyName}. Check-in is {booking.CheckInDate:dd MMM yyyy}.";

			// 1. In-app notification
			await _unitOfWork.DapperRepo.InsertSingleNotificationAsync(
				booking.UserId,
				title,
				body,
				NotificationType.BalancePaymentReminder,
				bookingRef
			);

			// 2. Push notification
			var deviceToken = await _unitOfWork.DapperRepo.GetUserActiveDeviceTokenAsync(booking.UserId);

			if (deviceToken.Any())
			{
				await _firebase.SendBulkAsync(
					deviceToken,
					isDueToday ? "Balance Due Today ⏰" : "Balance Payment Reminder 💳",
					body,
					new Dictionary<string, string>
					{
					{ "type",        NotificationType.BalancePaymentReminder },
					{ "referenceId", bookingRef }
					}
				);
			}
		}
	}
}
