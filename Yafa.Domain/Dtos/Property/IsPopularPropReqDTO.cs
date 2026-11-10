namespace Yafa.Domain.Dtos.Property;

public class IsPopularPropReqDTO
{
	public decimal? Latitude { get; set; }
	public decimal? Longitude { get; set; }
	public string LoggedInUserId { get; set; } = string.Empty;
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 10;
}
public class IsFeaturedPropReqDTO
{
	public decimal? Latitude { get; set; }
	public decimal? Longitude { get; set; }
	public string LoggedInUserId { get; set; } = string.Empty;
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 10;
}