using Amazon.S3.Model;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        public readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket([FromRoute] string bucketName)
        {
            var bucketExists = await _bucketRepository.S3BucketExistsAsync(bucketName);

            if (bucketExists)
            {
                return BadRequest($"S3 bucket named {bucketName} is already exists");
            }

            var resultFromCreateRequest = await _bucketRepository.CreateBucketAsync(bucketName);

            if (resultFromCreateRequest is null)
            {
                return BadRequest();
            }

            return Ok(resultFromCreateRequest);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<ListS3BucketResponse>>> ListS3Buckets()
        {
            var result = await _bucketRepository.GetAllS3BucketsAsync();

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteBucket(string bucketName)
        {
            await _bucketRepository.DeleteBucketAsync(bucketName);

            return Ok();
        }
    }
}
