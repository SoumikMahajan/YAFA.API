namespace Yafa.Domain.Dtos.Admin;

public class CalendarBookingRequestDTO
{
    public string startDate { get; set; } = string.Empty;
    public string endDate { get; set; } = string.Empty;
    public Guid PropertyPublicId { get; set; }
}

public class CalendarBookingDto
{
    public Guid BookingPublicId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid PropertyPublicId { get; set; } = Guid.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;   
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public string Color { get; set; } = string.Empty;

	public string? PaymentPlanType { get; set; }  
	public decimal? RemainingBalance { get; set; }
}