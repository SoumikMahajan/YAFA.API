using Yafa.Domain.Dtos;

namespace Yafa.Application.Common.Interfaces
{
    public interface IFileRepo
    {
        //Task<(string WebPath, string LocalPath, string FileName)> SaveBase64ImageAsync(string base64,string extension,string fileNamePrefix,string relativeFolderPath);
        Task<ImageUploadResultDTO> SaveBase64ImageAsync(Base64ImageSaveRequestDTO model);
    }
}
