namespace Yafa.Domain.Dtos.Property;

public class PropReviewsDTO
{
    public Guid ReviewPublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime? CheckInDate { get; set; }
    public int StayDuration { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int HelpfulCount { get; set; }
    public bool IsVerified { get; set; }
    public string ResponseFromOwner { get; set; } = string.Empty;
    public DateTime? ResponseDate { get; set; }   
}

public class PropRatingDetailsDTO
{
    public Guid PropertyPublicId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
    public decimal? FiveStarPercentage { get; set; }
    public decimal? FourStarPercentage { get; set; }
    public decimal? ThreeStarPercentage { get; set; }
    public decimal? TwoStarPercentage { get; set; }
    public decimal? OneStarPercentage { get; set; }
    public List<PropReviewsDTO>? propReviews { get; set; } = new();
}

public class PropRatingRequestDTO
{
    public Guid PropertyPublicId { get; set; }
	public decimal? Rating { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid BookingPublicId { get; set; }
    public string Comment { get; set; } = string.Empty;
	public DateOnly? CheckInDate { get; set; }
	public int StayDuration { get; set; }

}


