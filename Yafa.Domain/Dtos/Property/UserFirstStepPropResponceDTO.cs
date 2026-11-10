namespace Yafa.Domain.Dtos.Property;

public class UserFirstStepPropResponceDTO
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyType { get; set; } = string.Empty; // Villa, Apartment, Hotel, Resort
    public string PropertyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int TotalBedrooms { get; set; } = 0;
    public int TotalBathrooms { get; set; } = 0;
    public int MaxPersons { get; set; } = 0;
    public decimal? DiscountPercentage { get; set; }
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
	public string CreatedBy { get; set; } = string.Empty;
}

public class UserSecondStepPropResponceDTO
{
    public PropertyBasicDetailsDto BasicDetails { get; set; } = new();
    public List<PropertyAmenityDto> Amenities { get; set; } = new();
    public List<PropertyAdditionalServiceDto> AdditionalServices { get; set; } = new();
}

public class PropertyBasicDetailsDto
{
    public Guid PropertyPublicId { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string CheckInInstructions { get; set; } = string.Empty;
}

public class PropertyAmenityDto
{
    public Guid PropertyPublicId { get; set; }
    public int AmenityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string AmenityIcon { get; set; } = string.Empty;
}

public class PropertyAdditionalServiceDto
{
    public Guid PropertyPublicId { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
}



public class UserThirdStepPropResponceDTO
{
    public PropertyRefundPolicies RefundPolicyDetails { get; set; } = new();
    public List<PropertyRules> Rules { get; set; } = new();
    public List<PropertyPricingTiers> PriceingTiers { get; set; } = new();
}

public class PropertyRules
{
    public Guid PropertyPublicId { get; set; }
    public int RuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string RuleIcon { get; set; } = string.Empty;
}

public class PropertyRefundPolicies
{
    public Guid PropertyPublicId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string PolicyDescription { get; set; } = string.Empty;
    public decimal? RefundPercentage { get; set; }
    public int DaysBeforeCheckIn { get; set; }
}

public class PropertyPricingTiers
{
    public Guid PropertyPublicId { get; set; }
    public string TierType { get; set; } = string.Empty;
    public decimal? PricePerNight { get; set; }
}

public class UserFourthStepPropResponceDTO
{
    public List<PropertyImages> Images { get; set; } = new();
}

public class PropertyImages
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyImageName { get; set; } = string.Empty;
    public string PropertyImagePath { get; set; } = string.Empty;   
    public string ThumbPropertyImageName { get; set; } = string.Empty;
    public string ThumbPropertyImagePath { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}
