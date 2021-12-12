using LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;

namespace LifeBackup.Core.Interfaces
{
    public interface IFileRepository
    {
        Task<AddFileResponse> UploadFilesAsync(string bucketName, IList<IFormFile> filesToUpload);

        Task<IEnumerable<ListFilesResponse>> GetAllAsync(string bucketName);

        Task<DeleteFileResponse> DeleteFileAsync(string bucketName, string fileName);

        Task AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request);

        Task<GetJsonObjectResponse> GetJsonObjectAsync(string bucketName, string fileName);
    }
}
