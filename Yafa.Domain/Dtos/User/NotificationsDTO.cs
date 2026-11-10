namespace Yafa.Domain.Dtos.User;

public class NotificationsDTO
{
}

public class ActiveDeviceTokenDTO
{
	public Guid UserId { get; set; }
	public string FirebaseToken { get; set; } = string.Empty;
	public string? Platform { get; set; }
}

public class BroadcastNotificationResultDTO
{
	public int TotalTokens { get; set; }
	public int SuccessCount { get; set; }
	public int FailureCount { get; set; }
	public int InvalidTokensDeactivated { get; set; }
	public int BatchesProcessed { get; set; }
}

public class BroadcastNotificationRequestDTO
{
	public string Title { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	public string ReferencePublicId { get; set; } = string.Empty;
	//public Dictionary<string, string>? Data { get; set; }
}

public class NotificationResponseDTO
{
	public int NotificationId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public string Type { get; set; } = string.Empty;
	public string? ReferenceId { get; set; }
	public bool IsRead { get; set; }
	public DateTime? CreatedAt { get; set; }
}

public static class NotificationType
{
	public const string Booking = "BOOKING";
	public const string Offer = "OFFER";
	public const string CheckInReminder = "CHECKIN_REMINDER";
	public const string CheckOutReminder = "CHECKOUT_REMINDER";
	public const string Payment = "PAYMENT";
	public const string BalancePaymentReminder = "BALANCE_PAYMENT_REMINDER";
	public const string Review = "REVIEW";
	public const string System = "SYSTEM";
}

public class BalancePaymentReminderDto
{
	public Guid BookingPublicId { get; set; }
	public string UserId { get; set; } = string.Empty;
	public string BookingReference { get; set; } = string.Empty;
	public string PropertyName { get; set; } = string.Empty;
	public decimal RemainingBalance { get; set; }
	public DateOnly CheckInDate { get; set; }
}

public class CheckInReminderDto
{
	public Guid BookingPublicId { get; set; }
	public string UserId { get; set; } = string.Empty;
	public DateOnly? CheckInDate { get; set; }
	public DateOnly? CheckOutDate { get; set; }
	public string PropertyName { get; set; } = string.Empty;
	public TimeSpan Checkintime { get; set; }
	public TimeSpan Checkouttime { get; set; }
}