namespace Yafa.Domain.Dtos.Property
{
    public class NotificationRequestDTO
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
