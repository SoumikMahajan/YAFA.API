namespace Yafa.Domain.Dtos.Admin;

public class FinalSubmitPropertyResponceDTO
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;    
    public string Address { get; set; } = string.Empty;
    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? Rating { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string PolicyDescription { get; set; } = string.Empty;
    public decimal? Refundpercentage { get; set; }
    public decimal? MemberPricePerNight { get; set; }
    public decimal? NonMemberPricePerNight { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public string ThumbCoverImageUrl { get; set; } = string.Empty;
    public string ImagesUrl { get; set; } = string.Empty;
    public string ThumbImagesUrl { get; set; } = string.Empty;
    public bool IsPopular { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
	public string Description { get; set; } = string.Empty;
	public string ShortDescription { get; set; } = string.Empty;
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
	public string ActiveStatus { get; set; } = string.Empty;
	public string PropertyCreatedBy { get; set; } = string.Empty;
    public int TotalCount { get; set; } 
}

public class OwnerPropertyResponceDTO
{
	public Guid PropertyPublicId { get; set; }
	public string PropertyType { get; set; } = string.Empty;
	public string PropertyName { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public int? TotalBedrooms { get; set; }
	public int? TotalBathrooms { get; set; }
	public int? MaxPersons { get; set; }
	public TimeSpan? CheckInTime { get; set; }
	public TimeSpan? CheckOutTime { get; set; }
	public decimal? DiscountPercentage { get; set; }
	public decimal? Rating { get; set; }
	public string AmenityName { get; set; } = string.Empty;
	public string ServiceName { get; set; } = string.Empty;
	public string RuleName { get; set; } = string.Empty;
	public string PolicyDescription { get; set; } = string.Empty;
	public decimal? Refundpercentage { get; set; }
	public decimal? MemberPricePerNight { get; set; }
	public decimal? NonMemberPricePerNight { get; set; }
	public string CoverImageUrl { get; set; } = string.Empty;
	public string ThumbCoverImageUrl { get; set; } = string.Empty;
	public string ImagesUrl { get; set; } = string.Empty;
	public string ThumbImagesUrl { get; set; } = string.Empty;
	public decimal? Latitude { get; set; }
	public decimal? Longitude { get; set; }
	public string Description { get; set; } = string.Empty;
	public string ShortDescription { get; set; } = string.Empty;
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
}