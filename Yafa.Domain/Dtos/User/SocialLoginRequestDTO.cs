namespace Yafa.Domain.Dtos.User;

public class SocialLoginRequestDTO
{
    public string IdToken { get; set; } = string.Empty;
    public string provider { get; set; } = string.Empty;
    public string? DeviceToken { get; set; } = string.Empty;
    public string? DeviceTokenPlatform { get; set; } = string.Empty;
}

