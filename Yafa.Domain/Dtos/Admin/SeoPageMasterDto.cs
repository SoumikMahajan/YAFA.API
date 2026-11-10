namespace Yafa.Domain.Dtos.Admin;

public class SeoPageMasterDto
{
    public int PageKeyID { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}