namespace Yafa.Domain.Dtos.Property;

public class PropertyUnavailableDatesDTO
{
	public List<string> ConfirmedDates { get; set; } = new();
	public List<string> HoldDates { get; set; } = new();
	public List<string> BlockedDates { get; set; } = new();
}
