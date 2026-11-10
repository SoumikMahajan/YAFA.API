namespace Yafa.Domain.Dtos.Admin;

public class SeoContentDto
{
    public int SeoContentID { get; set; }
    public int PageKeyID { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? SefUrl { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}