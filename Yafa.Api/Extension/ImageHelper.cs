namespace Yafa.Api.Extension;
#pragma warning disable CS1591

public static class ImageHelper
{
    public static string? DetectImageExtension(byte[] bytes)
    {
        if (bytes.Length < 4) return null;

        // JPEG
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "jpg";

        // PNG
        if (bytes[0] == 0x89 && bytes[1] == 0x50 &&
            bytes[2] == 0x4E && bytes[3] == 0x47)
            return "png";

        // WebP
        if (bytes.Length >= 12 &&
            bytes[0] == 0x52 && bytes[1] == 0x49 &&
            bytes[2] == 0x46 && bytes[3] == 0x46 &&
            bytes[8] == 0x57 && bytes[9] == 0x45 &&
            bytes[10] == 0x42 && bytes[11] == 0x50)
            return "webp";

        return null;
    }
}
