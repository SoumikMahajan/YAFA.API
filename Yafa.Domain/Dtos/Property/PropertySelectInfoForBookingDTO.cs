using System.ComponentModel;

namespace Yafa.Domain.Dtos.Property;

public class PropertySelectInfoForBookingDTO
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public decimal? Rating { get; set; }
    public int? TotalReviews { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal PricePerNight { get; set; } 
    public decimal PricingDiscountPercentage { get; set; }
    public decimal EffectivePricePerNight { get; set; }
    public bool IsWishlisted { get; set; }
    public PropertyUnavailableDatesDTO? UnavailableDates { get; set; } = new();
}

public class BookingHoldRequestDTO
{
    public Guid PropertyPublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int numberOfGuests { get; set; }
}

public class BookingHoldResultDTO
{
    public Guid HoldPublicId { get; set; }
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? PropertyAddress { get; set; }
    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }

    // Booking summary
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int NumberOfGuests { get; set; }

    // Fee breakdown
    public decimal PricePerNight { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

	public decimal PartialDepositAmount { get; set; }
	public decimal PartialBalanceAmount { get; set; }

	// Hold timer
	public DateTime HoldExpiryTime { get; set; }
    public int SecondsRemaining { get; set; }

    public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();

}

public class BookingFeeDTO
{
    public Guid? BookingFeePublicId { get; set; } = Guid.Empty;
    public int FeeTypeId { get; set; }
    public string FeeName { get; set; } = string.Empty;
    public string FeeCategory { get; set; } = string.Empty;
    public decimal FeeValue { get; set; }
    public decimal CalculatedAmount { get; set; }
    public bool IsExtra { get; set; }
	public string? Notes { get; set; } = string.Empty;
}

public class InitiatePaymentRequest
{
    public Guid HoldPublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid? OfferPublicId { get; set; }

	[DefaultValue("Full")]
	public string PaymentPlanType { get; set; } = "Full";
}

public class InitiatePaymentResponseDto
{
    public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
	public int SecondsRemaining { get; set; }
	public string PaymentPlanType { get; set; } = "Full";
	public decimal BookingTotalAmount { get; set; }
	public decimal BookingAmountPaid { get; set; }
}

public class InitiateBalancePaymentRequest
{
	public Guid BookingPublicId { get; set; }
	public string UserId { get; set; } = string.Empty;
}

public class BalancePaymentResponseDto
{
	public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;

	/// <summary>Amount for this balance payment (TotalAmount - AmountPaid at the time of the call).</summary>
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = string.Empty;
	public string PaymentStatus { get; set; } = string.Empty;
	public decimal BookingTotalAmount { get; set; }
	public decimal BookingAmountPaid { get; set; }
}

public class StripeInitiateBalancePaymentResponseDto
{
	public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = "INR";
	public string ClientSecret { get; set; } = string.Empty;
	public string PaymentIntentId { get; set; } = string.Empty;
	public decimal BookingTotalAmount { get; set; }
	public decimal BookingAmountPaid { get; set; }
}

public class UpdateStripePaymentIntentRequestDTO
{
	public Guid PaymentPublicId { get; set; }
	public string PaymentIntentId { get; set; } = string.Empty;
	public string? CustomerId { get; set; } = string.Empty;
	public string StripeStatus { get; set; } = string.Empty;
	public string Currency { get; set; } = "INR";
	public string? ResponseJson { get; set; }
}

public class ConfirmPaymentRequest
{
    public Guid BookingPublicId { get; set; }
    public Guid HoldPublicId { get; set; }
    public string PaymentMethod { get; set; } = "Credit / Debit Card";

    // Mocked for now — real transaction ID comes from payment gateway callback
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}

// Response
public class ConfirmPaymentResponseDto
{
    public Guid BookingPublicId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? PropertyAddress { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    // Guest
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;

    // Booking summary
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int TotalGuests { get; set; }

    // Charges
    public decimal PricePerNight { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    // Payment
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Booking status
    public int BookingStatus { get; set; }
    public string BookingStatusName { get; set; } = string.Empty;
    public DateTime? ConfirmedDate { get; set; }

    public List<BookingFeeDTO> FeeBreakdown { get; set; } = new();
}

public class StripeWebhookResponseDto
{
    public bool IsNewlyConfirmed { get; set; }
	public bool IsPaymentFailed { get; set; }
	public string NotifyUserId { get; set; } = string.Empty;
    public string NotifyBookingPublicId { get; set; } = string.Empty;
	public string NotifyPropertyName { get; set; } = string.Empty;
	public string NotifyPropertyPublicId { get; set; } = string.Empty;
	public decimal? NotifyTotalAmount { get; set; }
	public DateOnly? NotifyCheckInDate { get; set; }

	public string NotifyPaymentType { get; set; } = string.Empty; // "Full" | "Deposit" | "Balance"
	public decimal? NotifyPaymentAmount { get; set; }
	public decimal? NotifyAmountPaid { get; set; }
	public DateOnly? NotifyBalanceDueDate { get; set; }
	public DateOnly? NotifyCheckOutDate { get; set; }

}
