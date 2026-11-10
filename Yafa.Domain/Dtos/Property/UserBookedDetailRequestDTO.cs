using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.Property;

public class UserBookedDetailRequestDTO
{
    public string UserId { get; set; } = string.Empty;
    public string? Search { get; set; } = string.Empty;
	public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class UserBookingListItemDto
{
    // Booking core
    public Guid BookingPublicId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int BookingStatus { get; set; }
    public string BookingStatusName { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime BookingDate { get; set; }

    // Property info
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? PropertyAddress { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    // Stay summary
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int TotalGuests { get; set; }

    // Total only
    public decimal? TotalAmount { get; set; }
    public decimal? RefundAmount { get; set; }

    public DateTime? RefundProcessedDate { get; set; }
    public decimal? PricePerNight { get; set; }

	public string PaymentPlanType { get; set; } = "Full";   // "Full" | "Partial"
	public decimal? RemainingBalance { get; set; }

	// Image
	public string? ThumbCoverImageUrl { get; set; }

    // Visibility flags
    public bool IsExtendButtonVisible { get; set; }
    public bool IsCancelBookingVisible { get; set; }
    public bool IsRebookingVisible { get; set; }
    public bool IsRatingAreaVisible { get; set; }
    public bool IsViewInvoiceVisible { get; set; }
	public bool IsPayBalanceVisible { get; set; }

	// Pagination meta
	public int TotalBookings { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class BookingDetailsResponceDto
{
    // Booking core
    public Guid BookingPublicId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int BookingStatus { get; set; }
    public string BookingStatusName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string? CancellationReason { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundProcessedDate { get; set; }
    public string? SpecialRequests { get; set; }

    // Guest info
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public string? GuestCountry { get; set; }

    // Property info
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public string? PropertyAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;

	// Stay details
	public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
	public DateTime? CheckInTime { get; set; }
	public DateTime? CheckOutTime { get; set; }
	public int TotalNights { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public int TotalGuests { get; set; }

    // Full charge breakdown
    public decimal PricePerNight { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

	public string PaymentPlanType { get; set; } = "Full";   // "Full" | "Partial"
	public decimal AmountPaid { get; set; }
	public decimal RemainingBalance { get; set; }
	public DateOnly? BalanceDueDate { get; set; }
	public bool CanPayBalance { get; set; }

	// live payment state
	//public Guid? PaymentPublicId { get; set; }
	//public string? PaymentGateway { get; set; } = string.Empty;
	//public string? CurrencyCode { get; set; } = string.Empty;
	//public decimal? PaymentAmount { get; set; } 
	//public string? PaymentStatus { get; set; } = string.Empty;
	//public DateTime? PaymentInitiatedDate { get; set; }
	//public DateTime? PaymentUpdatedDate { get; set; }

	// Stripe detail
	//public string? PaymentIntentId { get; set; } = string.Empty;
	//public string? ChargeId { get; set; } = string.Empty;
	//public string? PaymentMethodId { get; set; } = string.Empty;
	//public string? StripeStatus { get; set; } = string.Empty;

	public List<BookingPropertyImageDto> Images { get; set; } = new();
    public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();

	/// the actual pay-balance UI; the flat Payment* fields above are for legacy Full-plan consumers.</summary>
	public List<BookingPaymentSlotDto> PaymentPlan { get; set; } = new();
	//public List<PaymentTransactionHistoryDto>? PaymentHistory { get; set; } = new();
}

public class BookingPaymentSlotDto
{
	public Guid PaymentPublicId { get; set; }
	public string PaymentType { get; set; } = string.Empty;   // "Full" | "Deposit" | "Balance"
	public decimal Amount { get; set; }
	public string PaymentStatus { get; set; } = string.Empty;
	public DateTime? InitiatedDate { get; set; }
	public DateTime? UpdatedDate { get; set; }
	public string? CardBrand { get; set; }
	public string? CardLast4 { get; set; }
	//public string? PaymentIntentId { get; set; }
	//public string? ChargeId { get; set; }
	//public string? PaymentMethodId { get; set; }
	//public string? StripeStatus { get; set; }
}

public class BookingPropertyImageDto
{
    public string? PropertyImagePath { get; set; }
    public string? ThumbPropertyImagePath { get; set; }
    public bool IsCover { get; set; }
    public int? DisplayOrder { get; set; }
}

public class PaymentTransactionHistoryDto
{
	public Guid TransactionPublicId { get; set; }
	public string? TransactionType { get; set; } = string.Empty;
	public decimal? Amount { get; set; } 
	public string? TransactionStatus { get; set; } = string.Empty;
	public DateTime? CreatedDate { get; set; }
}

