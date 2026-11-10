namespace Yafa.Domain.Dtos.Admin;

public class SeoContentAddRequestDto
{
    public int PageKeyID { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? SefUrl { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}