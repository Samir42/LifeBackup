using LifeBackup.Core.Communication.Bucket;

namespace LifeBackup.Core.Interfaces
{
    public interface IBucketRepository
    {
        Task<bool> S3BucketExistsAsync(string bucketName);

        Task<CreateBucketResponse> CreateBucketAsync(string bucketName);

        Task<IEnumerable<ListS3BucketResponse>> GetAllS3BucketsAsync();

        Task DeleteBucketAsync(string bucketName);
    }
}
