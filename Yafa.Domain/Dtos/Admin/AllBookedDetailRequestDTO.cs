using System.ComponentModel;

namespace Yafa.Domain.Dtos.Admin;


public class AllBookedDetailRequestDTO
{
	public int BookingStatus { get; set; }
	[DefaultValue(1)]
	public int PageNumber { get; set; } = 1;
	[DefaultValue(10)]
	public int PageSize { get; set; } = 10;
	public string? SearchTerm { get; set; }
	public string? SortBy { get; set; }
	public string? SortDirection { get; set; }
	public string? StartDate { get; set; }
	public string? EndDate { get; set; }
	[DefaultValue(1)]
	public int PaymentStatus { get; set; }
}

public class AllBookedDetailResponseDTO
{
	public List<AdminBookingListItemDto> Bookings { get; set; } = new();
	public AdminBookingCount BookingCounts { get; set; } = new();
}

public class AdminBookingListItemDto
{
	// Booking core
	//[JsonIgnore]
	//public int BookingId { get; set; }
	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public string GuestName { get; set; } = string.Empty;
	public string PropertyName { get; set; } = string.Empty;
	public DateOnly CheckInDate { get; set; }
	public DateOnly CheckOutDate { get; set; }
	public decimal TotalAmount { get; set; }
	public int BookingStatus { get; set; }
	public string BookingStatusName { get; set; } = string.Empty;
	public string PaymentStatus { get; set; } = string.Empty;
	public int TotalBookings { get; set; }
	public int CurrentPage { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }


	//   public decimal? Amount { get; set; }
	//public DateTime BookingDate { get; set; }
	//   public DateTime? ConfirmedDate { get; set; }
	//   public string? SourceName { get; set; } = string.Empty;

	//// Guest info

	//   public string GuestEmail { get; set; } = string.Empty;
	//   public string? GuestPhone { get; set; }
	//   public string? GuestCountry { get; set; }

	//   // Property info
	//   public Guid PropertyPublicId { get; set; }

	//   public string? PropertyAddress { get; set; }
	//   public string? City { get; set; }
	//   public string? Country { get; set; }

	//   // Stay details

	//   public int TotalNights { get; set; }
	//   public int TotalGuests { get; set; }

	//   // Financials
	//   public decimal PricePerNight { get; set; }
	//   public decimal SubtotalAmount { get; set; }
	//   public decimal DiscountAmount { get; set; }


	//public string PaymentPlanType { get; set; } = "Full";
	//public decimal AmountPaid { get; set; }
	//public decimal RemainingBalance { get; set; }
	//public DateOnly? BalanceDueDate { get; set; }


	//// Pagination meta
	//public int TotalBookings { get; set; }
	//   public int CurrentPage { get; set; }
	//   public int PageSize { get; set; }
	//   public int TotalPages { get; set; }

	//   public List<AdminBookingFeeDTO>? FeeBreakdown { get; set; } = new();
}

//public class AdminBookingFeeDTO
//{
//    [JsonIgnore]
//    public int BookingId { get; set; }
//    public Guid BookingFeePublicId { get; set; }
//    public int FeeTypeId { get; set; }
//    public string FeeName { get; set; } = string.Empty;
//    public string FeeCategory { get; set; } = string.Empty;
//    public decimal FeeValue { get; set; }
//    public decimal CalculatedAmount { get; set; }
//    public bool IsExtra { get; set; }
//}

public class AdminBookingCount
{
	public int AllCount { get; set; }
	public int ConfirmedCount { get; set; }
	public int PendingCount { get; set; }
	public int CompletedCount { get; set; }
	public int CancelledCount { get; set; }
	public int CheckedIn { get; set; }
}