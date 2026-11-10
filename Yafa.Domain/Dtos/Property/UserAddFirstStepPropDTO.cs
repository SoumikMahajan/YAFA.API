using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.Property;

public class UserAddFirstStepPropDTO
{
    public Guid? PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty; // Villa, Apartment, Hotel, Resort
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
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

public class UserAddSecondStepPropDTO
{
    public Guid PropertyPublicId { get; set; }
    public TimeSpan CheckInTime { get; set; }
    public TimeSpan CheckOutTime { get; set; }
    public string CheckInInstructions { get; set; } = string.Empty;
    public string AmenityIds { get; set; } = string.Empty;
    public string ServiceIDs { get; set; } = string.Empty;
    public string ServicePrices { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class UserAddThirdStepPropDTO
{
    public Guid PropertyPublicId { get; set; }
    public string HouseRuleIds { get; set; } = string.Empty;
    public string RefundPolicyDescription { get; set; } = string.Empty;
    public decimal? RefundPercentage { get; set; }
    public int RefundDaysBeforeCheckIn { get; set; } = 0;
    public decimal? MemberPrice { get; set; }
    public decimal? NonMemberPrice { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class UserAddFourthStepPropDTO
{
    public Guid PropertyPublicId { get; set; }
    public List<UserAddPropertyImagesDTO> PropertyImages { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class UserAddPropertyImagesDTO
{
    public string ProfileImageBase64 { get; set; } = string.Empty;
    public string ProfileImageExtension { get; set; } = string.Empty;
}

public class PropertyImageDbDTO
{
    public string PropertyImageName { get; set; } = string.Empty;
    public string PropertyImagePath { get; set; } = string.Empty;
    public string PropertyImageLocalPath { get; set; } = string.Empty;
    public string ThumbPropertyImageName { get; set; } = string.Empty;
    public string ThumbPropertyImagePath { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateFullPropertyDTO
{
	public Guid? PropertyPublicId { get; set; }
	public string PropertyType { get; set; } = string.Empty;
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
	public TimeSpan CheckInTime { get; set; }
	public TimeSpan CheckOutTime { get; set; }
	public string CheckInInstructions { get; set; } = string.Empty;
	public string AmenityIds { get; set; } = string.Empty;
	public string ServiceIDs { get; set; } = string.Empty;
	public string ServicePrices { get; set; } = string.Empty;
	public string HouseRuleIds { get; set; } = string.Empty;
	public string RefundPolicyDescription { get; set; } = string.Empty;
	public decimal? RefundPercentage { get; set; }
	public int RefundDaysBeforeCheckIn { get; set; } = 0;
	public decimal? MemberPrice { get; set; }
	public decimal? NonMemberPrice { get; set; }
	public string CreatedBy { get; set; } = string.Empty;
}