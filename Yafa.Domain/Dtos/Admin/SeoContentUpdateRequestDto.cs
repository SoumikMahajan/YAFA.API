namespace Yafa.Domain.Dtos.Admin;

public class SeoContentUpdateRequestDto
{
    public int SeoContentID { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? SefUrl { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}