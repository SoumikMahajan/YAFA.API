using System.ComponentModel;
using Yafa.Domain.Dtos.Property;

namespace Yafa.Domain.Dtos.Admin;


public class CheckoutDetailsResponseDto
{
	public CheckoutBookingDetailsDto Booking { get; set; } = new ();
	public List<BookingFeeDTO>? AppliedFees { get; set; } = new();
	public List<AvailableFeeTypeDto>? AvailableFeeTypes { get; set; } = new();

	//public decimal FeesTotal => AppliedFees?.Sum(f => f.CalculatedAmount) ?? 0;
	//public decimal GrandTotal => (Booking?.SubtotalAmount ?? 0) + FeesTotal;
	//public decimal AmountPaid => Booking?.PaymentStatus == "Succeeded" ? (Booking?.PaymentAmount ?? 0) : 0;
	//public decimal RemainingPayment => Math.Max(0, GrandTotal - AmountPaid);
	//public bool IsFullyPaid => RemainingPayment <= 0;
}

public class AvailableFeeTypeDto
{
	public Guid FeePublicId { get; set; }
	public string FeeName { get; set; } = string.Empty;
	public string FeeCategory { get; set; } = string.Empty;
	public decimal? FeeValue { get; set; }
	public decimal? SuggestedAmount { get; set; }
}

public class CheckoutBookingDetailsDto
{
	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public int BookingStatus { get; set; }
	public string BookingStatusName { get; set; } = string.Empty;

	public DateTime? CheckInDate { get; set; }
	public DateTime? CheckOutDate { get; set; }
	public int TotalNights { get; set; }
	public int NumberOfAdults { get; set; }
	public int NumberOfChildren { get; set; }
	public int TotalGuests { get; set; }
	public decimal? PricePerNight { get; set; }
	public decimal? SubtotalAmount { get; set; }
	public decimal? DiscountAmount { get; set; }
	public decimal? TotalAmount { get; set; }

	public string PaymentPlanType { get; set; } = "Full";
	public decimal? AmountPaid { get; set; }
	public decimal FeesTotal { get; set; }
	public decimal GrandTotal { get; set; }
	public decimal RemainingPayment { get; set; }
	public bool IsFullyPaid { get; set; }

	public string GuestName { get; set; } = string.Empty;
	public string GuestEmail { get; set; } = string.Empty;
	public string GuestPhone { get; set; } = string.Empty;
	public string GuestCountry { get; set; } = string.Empty;

	public Guid PropertyPublicId { get; set; }
	public string PropertyName { get; set; } = string.Empty;
	public string PropertyAddress { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
}


public class AddExtraFeeRequestDto
{
	public Guid BookingPublicId { get; set; }
	public Guid FeePublicId { get; set; }
	public decimal? FeeAmount { get; set; }   // null => SP uses FeeTypes default calc
	public string Notes { get; set; } = string.Empty;
	public string CreatedBy { get; set; } = string.Empty;
}

public class RemoveExtraFeeRequestDto
{
	public Guid BookingFeePublicId { get; set; }
	public string CreatedBy { get; set; } = string.Empty;
}

public class CancelBookingRequestDTO
{
    public Guid BookingPublicId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class RefundBookingRequestDTO
{
    public Guid BookingPublicId { get; set; }
    public decimal RefundAmount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class ExtendBookingRequestDTO
{
    public Guid BookingPublicId { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string CreatedBy { get; set;} = string.Empty;
}

public class AdminAddNewBookingRequestDTO
{
    public Guid PropertyPublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
	public Guid BookingSourceType { get; set; }
    public Guid? OfferPublicId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class AdminAddNewBookingResponseDto
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
    public string PaymentGateway { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentAmount { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Booking status
    public int BookingStatus { get; set; }
    public string BookingStatusName { get; set; } = string.Empty;
    public DateTime? ConfirmedDate { get; set; }

    public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();
}

public sealed class AddManualPaymentRequestDto
{
	public Guid BookingPublicId { get; set; }
	public decimal Amount { get; set; }

	[DefaultValue("Cash")]
	public string PaymentMethod { get; set; } = "Cash";
	public string CreatedBy { get; set; } = string.Empty;
}
