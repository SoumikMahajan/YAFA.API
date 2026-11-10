namespace Yafa.Domain.Dtos.Admin;

public class AdminPropertyViewDetailsDto
{
	// Header
	public Guid PropertyPublicId { get; set; }
	public string PropertyName { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string? ZipCode { get; set; }
	public string Country { get; set; } = string.Empty;
	public string? ShortDescription { get; set; }
	public string? Description { get; set; }
	public bool IsActive { get; set; }
	public string PropertyType { get; set; } = string.Empty;   // change to int if column is an id
	public decimal? PricePerNight { get; set; }                // null if no Non-Member tier
	public int TotalBedrooms { get; set; }
	public int TotalBathrooms { get; set; }
	public int MaxPersons { get; set; }
	public TimeSpan CheckInTime { get; set; }                  // SQL time -> TimeSpan
	public TimeSpan CheckOutTime { get; set; }
	public DateTime ListedSince { get; set; }
	public decimal? Latitude { get; set; }
	public decimal? Longitude { get; set; }
	public decimal? MemberPrice { get; set; }
	public decimal? DiscountPercentage { get; set; }
	public string CheckInInstructions { get; set; } = string.Empty;
	public string RefundPolicyDescription { get; set; } = string.Empty;
	public decimal? RefundPercentage { get; set; }
	public int RefundDaysBeforeCheckIn { get; set; }
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;

	// Owner
	public string OwnerId { get; set; } = string.Empty;                        // int if Users.UserId is int
	public string? OwnerName { get; set; }
	public DateTime OwnerSince { get; set; }
	public int OwnerPropertyCount { get; set; }

	// Performance
	public int OccupancyRate { get; set; }                     // 0-100
	public decimal? AvgRating { get; set; }                    // null = no reviews
	public int ReviewCount { get; set; }

	public int TotalImageCount { get; set; }

	public List<AdminPropertyViewImageDto> Images { get; set; } = new();
	public List<AdminPropertyViewAmenityDto> Amenities { get; set; } = new();
	public List<AdminPropertyViewHouseRuleDto> HouseRules { get; set; } = new();
	public List<AdminPropertyViewBookingDto> RecentBookings { get; set; } = new();
}

public class AdminPropertyViewImageDto
{
	public Guid ImagePublicId { get; set; }
	public string? PropertyImageName { get; set; }
	public string PropertyImagePath { get; set; } = string.Empty;
}

public class AdminPropertyViewAmenityDto
{
	public Guid PropertyAmenityPublicId { get; set; }
	public int AmenityId { get; set; }
	public string AmenityName { get; set; } = string.Empty;
	public string? AmenityIcon { get; set; }
}

public class AdminPropertyViewHouseRuleDto
{
	public Guid PropertyRulePublicId { get; set; }
	public int RuleId { get; set; }
	public string RuleName { get; set; } = string.Empty;
	public string? RuleIcon { get; set; }
}

public class AdminPropertyViewBookingDto
{
	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public string? GuestName { get; set; }
	public DateTime CheckInDate { get; set; }
	public DateTime CheckOutDate { get; set; }
	public decimal TotalAmount { get; set; }
	public string StatusName { get; set; } = string.Empty;
}