namespace Yafa.Domain.Dtos.Admin;

public class PropertyImagesOnAdminPanelDTO
{
    public Guid PropertyPublicId { get; set; }
    public Guid ImagePublicId { get; set; }
    public string PropertyImageName { get; set; } = string.Empty;
    public string PropertyImagePath { get; set; } = string.Empty;
    public string ThumbPropertyImageName { get; set; } = string.Empty;
    public string ThumbPropertyImagePath { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}

public class AdminAddPropertyImagesDTO
{
    public Guid PropertyPublicId { get; set; }
    public List<ImagesDTO> PropertyImages { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class ImagesDTO
{
    public string ProfileImageBase64 { get; set; } = string.Empty;
}