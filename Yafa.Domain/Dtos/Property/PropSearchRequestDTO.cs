namespace Yafa.Domain.Dtos.Property;

public class PropSearchRequestDTO
{
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int Guests { get; set; }
    public string AmenityIds { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
public class PropSearchResponseDTO
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;   
    public string Address { get; set; } = string.Empty;
    public int TotalBedrooms { get; set; }
    public int TotalBathrooms { get; set; }
    public int MaxPersons { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal? Rating { get; set; }
    public decimal DistanceInMeters { get; set; }
    public string DistanceDisplay { get; set; } = string.Empty;
    public string AmenityName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string PolicyDescription { get; set; } = string.Empty;
    public decimal? Refundpercentage { get; set; }
    public int DaysBeforeCheckIn { get; set; }
    public decimal? MemberPricePerNight { get; set; }
    public decimal? NonMemberPricePerNight { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public string ThumbCoverImageUrl { get; set; } = string.Empty;
    public string ImagesUrl { get; set; } = string.Empty;
    public string ThumbImagesUrl { get; set; } = string.Empty;
    public bool IsPropertyWishlised { get; set; }
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
	public int TotalCount { get; set; }
    
}


public class PropertyDetailsDto
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }

    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }

    public decimal? Rating { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public bool IsPropertyWishlised { get; set; }

	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;

	public List<PropertyImages>? Images { get; set; } = new();
    public List<PropertyAmenityDto>? Amenities { get; set; } = new();
    public List<PropertyAdditionalServiceDto>? AdditionalServices { get; set; } = new();
    public List<PropertyRules>? HouseRules { get; set; } = new();
    public List<PropertyRefundPolicies>? RefundPolicy { get; set; } = new();
    public List<PropertyPricingTiers>? PricingTiers { get; set; } = new();
    public PropRatingDetailsDTO? propRatingDetails { get; set; } = new();
}
