using Yafa.Domain.Dtos.Property;

namespace Yafa.Domain.Dtos.Admin;

public class AdminBookingDetailsDto
{
	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public int BookingStatus { get; set; }
	public string BookingStatusName { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }

	// Guest
	public string GuestName { get; set; } = string.Empty;
	public string GuestEmail { get; set; } = string.Empty;
	public string? GuestPhone { get; set; }

	// Property
	public Guid PropertyPublicId { get; set; }
	public string PropertyName { get; set; } = string.Empty;
	public string? PropertyAddress { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Country { get; set; }

	// Stay
	public DateOnly CheckInDate { get; set; }
	public DateOnly CheckOutDate { get; set; }
	public int TotalNights { get; set; }
	public int NumberOfAdults { get; set; }
	public int NumberOfChildren { get; set; }
	public int TotalGuests { get; set; }

	// Payment breakdown
	//public string Currency { get; set; } = "AED";
	public decimal PricePerNight { get; set; }
	public decimal SubtotalAmount { get; set; }
	public string? PromoCode { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal TotalAmount { get; set; }

	// Payment panel
	//public string PaymentPlanType { get; set; } = "Full";
	//public string? PaymentGateway { get; set; }
	//public string? CardBrand { get; set; }
	//public string? CardLast4 { get; set; }
	//public string PaymentStatus { get; set; } = string.Empty;   // Paid | Deposit Paid | Unpaid | Refunded
	//public DateOnly? BalanceDueDate { get; set; }
	public decimal AmountPaid { get; set; }
	public decimal RemainingBalance { get; set; }
	public decimal VatPercent { get; set; }
	public decimal VatAmount { get; set; }
	public decimal AmountExclVat { get; set; }


	// Cancellation / refund
	public string? CancellationReason { get; set; }
	public string? CancellationBy { get; set; }
	public decimal? RefundAmount { get; set; }

	public string? InternalNotes { get; set; }

	public List<BookingFeeDTO> FeeBreakdown { get; set; } = new();
	public List<AdminBookingPaymentSlotDto> PaymentPlan { get; set; } = new();
	public List<AdminBookingTimelineDto> Timeline { get; set; } = new();
}

public class AdminBookingPaymentSlotDto
{
	public Guid PaymentPublicId { get; set; }
	public string PaymentType { get; set; } = string.Empty;   // Full | Deposit | Balance
	public decimal Amount { get; set; }
	public string PaymentStatus { get; set; } = string.Empty;
	public string PaymentGateway { get; set; } = string.Empty;
	public string CardBrand { get; set; } = string.Empty;
	public string CardLast4 { get; set; } = string.Empty;
	public DateTime PaymentDate { get; set; }
}

public class AdminBookingTimelineDto
{
	public string EventType { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public DateTime EventDate { get; set; }
	public bool IsCompleted { get; set; }
}

public class AdminBookingPriceSummery
{
	public int TotalNights { get; set; }
	public decimal? PricePerNight { get; set; }
	public decimal? SubtotalAmount { get; set; }
	public decimal? DiscountAmount { get; set; }
	public decimal? AmountExclVat { get; set; }
	public decimal? VatPercent { get; set; }
	public decimal? VatAmount { get; set; }
	public decimal? TotalAmount { get; set; }
	public decimal? DueNow { get; set; }
	public decimal? RemainingBalance { get; set; }
	public List<BookingFeeDTO> FeeBreakdown { get; set; } = new();
}

//public class AdminBookingDetailsDto
//{
//	public Guid BookingPublicId { get; set; }
//	public string BookingReference { get; set; } = string.Empty;
//	public int BookingStatus { get; set; }
//	public string BookingStatusName { get; set; } = string.Empty;
//	public string PaymentGateway { get; set; } = string.Empty;
//	public string PaymentStatus { get; set; } = string.Empty;
//	public decimal? PaymentAmount { get; set; }
//	public DateTime BookingDate { get; set; }
//	public DateTime? ConfirmedDate { get; set; }
//	public DateTime? CancellationDate { get; set; }
//	public string? CancellationReason { get; set; }
//	public string? CancellationBy { get; set; }
//	public decimal? RefundAmount { get; set; }
//	public DateTime? RefundProcessedDate { get; set; }
//	public string? SpecialRequests { get; set; }
//	public string? InternalNotes { get; set; }

//	// Guest info
//	public string GuestName { get; set; } = string.Empty;
//	public string GuestEmail { get; set; } = string.Empty;
//	public string? GuestPhone { get; set; }
//	public string? GuestCountry { get; set; }

//	// Property info
//	public Guid PropertyPublicId { get; set; }
//	public string PropertyName { get; set; } = string.Empty;
//	public string? PropertyType { get; set; }
//	public string? PropertyAddress { get; set; }
//	public string? City { get; set; }
//	public string? State { get; set; }
//	public string? Country { get; set; }
//	public int? TotalBedrooms { get; set; }
//	public int? TotalBathrooms { get; set; }
//	public int? MaxPersons { get; set; }

//	// Stay details
//	public DateOnly CheckInDate { get; set; }
//	public DateOnly CheckOutDate { get; set; }
//	public int TotalNights { get; set; }
//	public int NumberOfAdults { get; set; }
//	public int NumberOfChildren { get; set; }
//	public int TotalGuests { get; set; }

//	// Full charge breakdown
//	public decimal PricePerNight { get; set; }
//	public decimal SubtotalAmount { get; set; }
//	public decimal DiscountAmount { get; set; }
//	public decimal TotalAmount { get; set; }

//	public string PaymentPlanType { get; set; } = "Full";
//	public decimal AmountPaid { get; set; }
//	public decimal RemainingBalance { get; set; }
//	public DateOnly? BalanceDueDate { get; set; }
//	public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();
//	public List<AdminBookingPaymentSlotDto> PaymentPlan { get; set; } = new();
//}

//public class AdminBookingPaymentSlotDto
//{
//	public Guid PaymentPublicId { get; set; }
//	public string PaymentType { get; set; } = string.Empty;   // "Full" | "Deposit" | "Balance"
//	public decimal Amount { get; set; }
//	public string PaymentStatus { get; set; } = string.Empty;
//	public DateTime? InitiatedDate { get; set; }
//	public DateTime? UpdatedDate { get; set; }
//	public string? PaymentIntentId { get; set; }
//	public string? ChargeId { get; set; }
//	public string? PaymentMethodId { get; set; }
//	public string? StripeStatus { get; set; }
//}
