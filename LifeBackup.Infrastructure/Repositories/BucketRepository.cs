using Amazon.S3;
using Amazon.S3.Model;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;

namespace LifeBackup.Infrastructure.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<CreateBucketResponse> CreateBucketAsync(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true,
            };

            var response = await _s3Client.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse
            {
                RequestId = response.ResponseMetadata.RequestId,
                BucketName = bucketName
            };
        }

        public async Task DeleteBucketAsync(string bucketName)
        {
            await _s3Client.DeleteBucketAsync(bucketName);
        }

        public async Task<IEnumerable<ListS3BucketResponse>> GetAllS3BucketsAsync()
        {
            var s3Buckets = await _s3Client.ListBucketsAsync();

            return s3Buckets.Buckets.Select(x => new ListS3BucketResponse
            {
                BucketName = x.BucketName,
                CreationDate = x.CreationDate
            });
        }

        public async Task<bool> S3BucketExistsAsync(string bucketName)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketName);
        }
    }
}
