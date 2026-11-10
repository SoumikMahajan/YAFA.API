namespace Yafa.Domain.Dtos.Property;

public class CheckoutCouponDto
{
    public Guid OfferPublicId { get; set; }
    public string Title { get; set; } = string.Empty;

    public string? PriceLabel { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }

    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }
    public string? OfferImagePath { get; set; }
    public string? OfferThumbImagePath { get; set; }

    public decimal EstimatedDiscount { get; set; }
    public decimal FinalTotalAfterCoupon { get; set; }

    public string Terms { get; set; } = string.Empty;
}

public class CouponOfferResponceDto
{
    public Guid OfferPublicId { get; set; }
    public string Title { get; set; } = string.Empty;

    public string? PriceLabel { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }

    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }
    public string? OfferImagePath { get; set; }
    public string? OfferThumbImagePath { get; set; }

    public decimal EstimatedDiscount { get; set; }

    public string Terms { get; set; } = string.Empty;
}
