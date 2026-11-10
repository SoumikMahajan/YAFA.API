namespace Yafa.Domain.Dtos.Property;

public class UserCancelBookingRequestDto
{
	public Guid BookingPublicId { get; set; }
	public string CancellationReason { get; set; } = string.Empty;
	public string CancelBy { get; set; } = string.Empty;
}

public class UserExtendBookingRequestDto
{
	public Guid BookingPublicId { get; set; }
	public string? NewCheckOutDate { get; set; } 
	public int? NewNumberOfGuests { get; set; }
	public string ExtendBy { get; set; } = string.Empty;
}

public class UserExtendBookingResponseDto
{
	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public DateOnly CheckInDate { get; set; }
	public DateOnly CheckOutDate { get; set; }
	public int TotalNights { get; set; }
	public int TotalGuests { get; set; }
	public decimal PricePerNight { get; set; }
	public decimal SubtotalAmount { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal TotalAmount { get; set; }
	public int ExtraNightsAdded { get; set; }

    public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();
}