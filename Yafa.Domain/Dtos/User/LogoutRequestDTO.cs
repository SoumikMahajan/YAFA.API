namespace Yafa.Domain.Dtos.User
{
    public class LogoutRequestDTO
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
