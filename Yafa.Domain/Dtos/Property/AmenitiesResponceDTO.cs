namespace Yafa.Domain.Dtos.Property;

public class AmenitiesResponceDTO
{
    public int AmenityId { get; set; }
    public Guid AmenityPublicId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string AmenityIcon { get; set; } = string.Empty;
}

public class AdditionalServicesResponceDTO
{
    public int ServiceId { get; set; }
    public Guid ServicePublicId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
}

public class HouseRulesResponceDTO
{
    public int RuleId { get; set; }
    public Guid RulePublicId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string RuleIcon { get; set; } = string.Empty;
}
