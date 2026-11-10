namespace Yafa.Domain.Dtos.Admin;

public class SeoPageMasterUpdateRequestDto
{
    public int PageKeyID { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}