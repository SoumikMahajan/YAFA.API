using Yafa.Domain.Dtos.Property;

namespace Yafa.Domain.Dtos.Admin;

public class AdminInsertFullPropertyDTO
{
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
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
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
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
    public List<UserAddPropertyImagesDTO> PropertyImages { get; set; } = [];
    public string CreatedBy { get; set; } = string.Empty;
    public string OwnerPublicId { get; set; } = string.Empty;
}
