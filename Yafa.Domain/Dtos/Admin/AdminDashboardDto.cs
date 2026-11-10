namespace Yafa.Domain.Dtos.Admin;

public class AdminDashboardDto
{
	public CountStatCardDto TotalProperties { get; set; } = new();
	public CountStatCardDto TotalBookings { get; set; } = new();
	public CountStatCardDto PendingBookings { get; set; } = new();
	public CountStatCardDto TotalUsers { get; set; } = new();
	public AmountStatCardDto Revenue { get; set; } = new();
	public List<BookingStatusItemDto> BookingStatus { get; set; } = new();
	/// <summary>
	/// Gets or sets the recent bookings for the admin dashboard.
	/// </summary>public IEnumerable<AdminBookingListItemDto> RecentBookings { get; set; } = Enumerable.Empty<AdminBookingListItemDto>();
}

public abstract class StatCardBaseDto
{
	/// <summary>Null when there is no baseline (previous period value is 0).</summary>
	public decimal? ChangePercent { get; set; }
	/// <summary>up | down | flat</summary>
	public string ChangeDirection { get; set; } = "flat";
	/// <summary>week | month</summary>
	public string ComparisonPeriod { get; set; } = "month";
}

public class CountStatCardDto : StatCardBaseDto
{
	public int Value { get; set; }
	public int CurrentPeriodValue { get; set; }
	public int PreviousPeriodValue { get; set; }
}

public class AmountStatCardDto : StatCardBaseDto
{
	public decimal Value { get; set; }
	public decimal CurrentPeriodValue { get; set; }
	public decimal PreviousPeriodValue { get; set; }
}

public class BookingStatusItemDto
{
	/// <summary>Confirmed | Pending | Cancelled</summary>
	public string Status { get; set; } = string.Empty;
	public int Count { get; set; }
	public decimal Percentage { get; set; }
}

public class RevenueTrendItemDto
{
	public DateTime MonthStart { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }
	public string Label { get; set; } = string.Empty; 
	public decimal Revenue { get; set; }
	public int Bookings { get; set; }
	public bool IsCurrentMonth { get; set; }
}