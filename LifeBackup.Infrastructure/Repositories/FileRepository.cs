using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LifeBackup.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IAmazonS3 _s3Client;

        public FileRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task DownloadFileAsync(string bucketName, string fileName)
        {
            var path = $"C:\\S3Downloads\\{fileName}";

            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = path
            };

            using(var transferUtility  = new TransferUtility(_s3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }

        public async Task<IEnumerable<ListFilesResponse>> GetAllAsync(string bucketName)
        {
            var responseFromAWS = await _s3Client.ListObjectsAsync(bucketName);

            return responseFromAWS.S3Objects.Select(x => new ListFilesResponse
            {
                BucketName = x.BucketName,
                Key = x.Key,
                Owner = x.Owner.DisplayName,
                Size = x.Size
            });
        }

        public async Task<AddFileResponse> UploadFilesAsync(string bucketName, IList<IFormFile> filesToUpload)
        {
            var preSignedUrls = new List<string>();

            foreach (var file in filesToUpload)
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = file.OpenReadStream(),
                    Key = file.FileName,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using (var fileTransferUtility = new TransferUtility(_s3Client))
                {
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                var expiryUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = file.FileName,
                    Expires = DateTime.UtcNow.AddDays(1)
                };

                var url = _s3Client.GetPreSignedURL(expiryUrlRequest);

                preSignedUrls.Add(url);
            }

            return new AddFileResponse
            {
                PreSignedUrls = preSignedUrls
            };
        }

        public async Task<DeleteFileResponse> DeleteFileAsync(string bucketName, string fileName)
        {
            var multiObjectDeleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName
            };

            multiObjectDeleteRequest.AddKey(fileName);

            var responseFromAWS = await _s3Client.DeleteObjectsAsync(multiObjectDeleteRequest);

            return new DeleteFileResponse
            {
                NumberOfDeletedObjects = responseFromAWS.DeletedObjects.Count
            };
        }

        public async Task AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request)
        {
            var createdOnUtc = DateTime.UtcNow;

            var s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:dd}/{request.Id}";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = JsonConvert.SerializeObject(request)
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<GetJsonObjectResponse> GetJsonObjectAsync(string bucketName, string fileName)
        {
            var getJsonRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(getJsonRequest);

            using (var reader = new StreamReader(response.ResponseStream))
            {
                var content = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<GetJsonObjectResponse>(content);
            }
        }
    }
}
