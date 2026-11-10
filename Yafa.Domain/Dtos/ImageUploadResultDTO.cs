namespace Yafa.Domain.Dtos
{
    public class ImageUploadResultDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string WebPath { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;

        public string ThumbnailWebPath { get; set; } = string.Empty;
        public string ThumbnailImageName { get; set; } = string.Empty;
    }
}
