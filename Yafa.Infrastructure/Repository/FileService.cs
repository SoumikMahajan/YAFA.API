using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos;

namespace Yafa.Infrastructure.Repository
{
    public class FileService : IFileRepo
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogService _logService;

        public FileService(IWebHostEnvironment env, ILogService logService)
        {
            _env = env;
            _logService = logService;
        }

        //public async Task<(string WebPath, string LocalPath, string FileName)> SaveBase64ImageAsync(string base64, string extension, string fileNamePrefix, string relativeFolderPath)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(base64))
        //            throw new ArgumentException("Image data is empty.");

        //        if (base64.Contains(","))
        //            base64 = base64.Split(',')[1];

        //        byte[] imageBytes = Convert.FromBase64String(base64);

        //        if (imageBytes.Length > 2 * 1024 * 1024)
        //            throw new InvalidOperationException("Image size exceeds 2MB.");

        //        extension = extension.ToLower();
        //        if (!new[] { "jpg", "jpeg", "png" }.Contains(extension))
        //            throw new InvalidOperationException("Invalid image format.Please Upload jpg or jpeg or png fiel format");

        //        string rootPath = _env.WebRootPath;
        //        string folderPath = Path.Combine(rootPath, relativeFolderPath);

        //        if (!Directory.Exists(folderPath))
        //            Directory.CreateDirectory(folderPath);

        //        string fileName = $"{fileNamePrefix}.{extension}";
        //        string localPath = Path.Combine(folderPath, fileName);

        //        await File.WriteAllBytesAsync(localPath, imageBytes);

        //        string webPath = Path.Combine(relativeFolderPath, fileName)
        //                .Replace("\\", "/");

        //        return (webPath, localPath, fileName);

        //    }
        //    catch (FormatException ex)
        //    {
        //        await _logService.LogCustomAsync($"Image format error: {ex.Message}");
        //        throw; // middleware maps to 400
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        await _logService.LogCustomAsync($"Image format error: {ex.Message}");
        //        throw; // middleware maps to 409
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logService.LogCustomAsync($"Image format error: {ex.Message}");
        //        throw; // middleware maps to 500
        //    }
        //}

        public async Task<ImageUploadResultDTO> SaveBase64ImageAsync(Base64ImageSaveRequestDTO model)
        {
            var result = new ImageUploadResultDTO();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Base64Image))
                    throw new ArgumentException("Image data is empty.");

                var base64Data = model.Base64Image.Contains(",")
                        ? model.Base64Image.Split(',')[1]
                        : model.Base64Image;

                var imageBytes = Convert.FromBase64String(base64Data);

                if (imageBytes.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("Image size exceeds 2MB.");

                model.Extension = model.Extension.ToLower();
                if (!new[] { "jpg", "jpeg", "png", "webp" }.Contains(model.Extension))
                    throw new InvalidOperationException("Invalid image format.Please Upload jpg or jpeg or png fiel format");

                var fileName = $"{model.FileName}.{model.Extension}";
                result.FileName = fileName;

                // MAIN IMAGE
                var mainFolder = Path.Combine(_env.WebRootPath, model.ImageFolderPath);
                Directory.CreateDirectory(mainFolder);

                var mainLocalPath = Path.Combine(mainFolder, fileName);

                if (File.Exists(mainLocalPath))
                {
                    File.Delete(mainLocalPath);
                }

                await File.WriteAllBytesAsync(mainLocalPath, imageBytes);

                result.LocalPath = mainLocalPath;
                result.WebPath = Path.Combine(model.ImageFolderPath, fileName).Replace("\\", "/");

                // THUMBNAIL
                if (!string.IsNullOrWhiteSpace(model.ThumbnailFolderPath))
                {
                    var thumbFolder = Path.Combine(_env.WebRootPath, model.ThumbnailFolderPath);
                    Directory.CreateDirectory(thumbFolder);

                    var thumbFileName = $"ti_{fileName}";
                    var thumbLocalPath = Path.Combine(thumbFolder, thumbFileName);

                    if (File.Exists(thumbLocalPath))
                    {
                        File.Delete(thumbLocalPath);
                    }

                    await CreateThumbnailAsync(imageBytes, thumbLocalPath, model.Extension);

                    result.ThumbnailImageName = thumbFileName;
                    result.ThumbnailWebPath = Path
                        .Combine(model.ThumbnailFolderPath, thumbFileName)
                        .Replace("\\", "/");
                }

            }
            catch (FormatException ex)
            {
                await _logService.LogCustomAsync($"Image format error: {ex.Message}", "ERROR");
                throw new ApplicationException("Invalid image format.");
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogCustomAsync($"Image format error: {ex.Message}", "ERROR");
                throw;
            }
            catch (Exception ex)
            {
                await _logService.LogCustomAsync($"Image format error: {ex.Message}", "ERROR");
                throw;
            }

            return result;
        }

        private async Task CreateThumbnailAsync(byte[] imageBytes, string outputPath,string extension)
        {
            using var inputStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(200, 200),
                Mode = ResizeMode.Max
            }));

            if (extension == "jpg" || extension == "jpeg")
            {
                var jpegEncoder = new JpegEncoder
                {
                    Quality = 70
                };

                await image.SaveAsync(outputPath, jpegEncoder);
            }
            else if (extension == "png")
            {
                var pngEncoder = new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.Level6
                };

                await image.SaveAsync(outputPath, pngEncoder);
            }
            else if (extension == "webp")
            {
                var webpEncoder = new WebpEncoder { Quality = 75 };
                await image.SaveAsync(outputPath, webpEncoder);
            }
            else
            {
                await image.SaveAsync(outputPath);
            }
          
        }
    }
}
