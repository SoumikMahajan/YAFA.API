namespace Yafa.Domain.Dtos.Property;

public class WishListResponceDTO
{
    public Guid WishlistPublicId { get; set; }
    public DateTime AddedToWishlistDate { get; set; }
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Rating { get; set; }
    public int TotalReviews { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int TotalBedrooms { get; set; }
    public int TotalBathrooms { get; set; }
    public int MaxPersons { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public decimal? NonMemberPricePerNight { get; set; }
    public decimal? MemberPricePerNight { get; set; }
    public string Amenities { get; set; } = string.Empty;
	public string ContactFirstName { get; set; } = string.Empty;
	public string ContactLastName { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string ContactPhone { get; set; } = string.Empty;
	public int TotalWishlistItems { get; set; }
}
public class WishListRequestDTO
{
    public string UserId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AddRemoveWishListPropertyDTO
{
    public Guid PropertyPublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsAdding { get; set; }
}
