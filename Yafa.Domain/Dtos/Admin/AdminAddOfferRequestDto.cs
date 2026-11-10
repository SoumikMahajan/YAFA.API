namespace Yafa.Domain.Dtos.Admin;

public class AdminAddOfferRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string YafaCode { get; set; } = string.Empty;
	public string? PriceLabel { get; set; }
    public string? Description { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }
    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }
    public string? OfferImageBase64 { get; set; }
    public string OfferImageExtension { get; set; } = "jpg";
    public int SortOrder { get; set; } = 0;
    public List<string> Terms { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class AdminUpdateOfferRequestDto
{
    public Guid OfferPublicId { get; set; }
    public string? Title { get; set; }
    public string? PriceLabel { get; set; }
    public string? Description { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }
    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }

    public string? OfferImageBase64 { get; set; }
    public string OfferImageExtension { get; set; } = "jpg";

    public int? SortOrder { get; set; }

    public List<string>? Terms { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}

public class OfferListItemDto
{
    public Guid OfferPublicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string YafaCode { get; set; } = string.Empty;
	public int CouponUsageCount { get; set; }
	public string? PriceLabel { get; set; }
    public string? Description { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }
    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }
    public string? OfferImageName { get; set; }
    public string? OfferImagePath { get; set; }
    public string? OfferThumbImageName { get; set; }
    public string? OfferThumbImagePath { get; set; }
    public int SortOrder { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public string TermsAggregated { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class OfferDetailDto
{
    public Guid OfferPublicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PriceLabel { get; set; }
    public string? Description { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }
    public decimal? DiscountFixedAmount { get; set; }
    public string? DiscountNote { get; set; }
    public DateOnly? AvailableUntil { get; set; }
    public string? OfferImageName { get; set; }
    public string? OfferImagePath { get; set; }
    public string? OfferThumbImageName { get; set; }
    public string? OfferThumbImagePath { get; set; }
    public int SortOrder { get; set; }

    // Admin-only
    public bool? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    public List<OfferTermDto> Terms { get; set; } = new();
}

public class OfferTermDto
{
    public int OfferTermId { get; set; }
    public string TermText { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}