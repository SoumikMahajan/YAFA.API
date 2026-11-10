using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos
{
    public class Base64ImageSaveRequestDTO
    {
        public string Base64Image { get; set; } = string.Empty;
        public string Extension { get; set; } = "jpg";
        public string FileName { get; set; } = string.Empty;

        // Main image path (required)
        public string ImageFolderPath { get; set; } = string.Empty;

        // Thumbnail path (optional)
        public string? ThumbnailFolderPath { get; set; }

        // Optional thumbnail size
        public int ThumbnailWidth { get; set; } = 200;
        public int ThumbnailHeight { get; set; } = 200;
    }
}
