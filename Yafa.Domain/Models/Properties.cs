namespace Yafa.Domain.Models;

public class Properties
{
    public int PropertyId { get; set; }
    public Guid PropertyPublicId { get; set; }
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
    public int? TotalBedrooms { get; set; }
    public int? TotalBathrooms { get; set; }
    public int? MaxPersons { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string CheckInInstructions { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public int? TotalReviews { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdateDate { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;
    public bool IsDelete { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string DeletedBy { get; set; } = string.Empty;       
    public bool FinalSubmit { get; set; }
}
