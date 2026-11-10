namespace Yafa.Domain.Dtos.Property
{
    public class TopLocationDto
    {
        public Guid LocationPublicId { get; set; }
        public string City { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public string ThumbImagePath { get; set; } = string.Empty;
        public string ThumbImageName { get; set; } = string.Empty;
    }
}
