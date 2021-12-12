using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace LifeBackup.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FillesController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;

        public FillesController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }


        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> UploadFiles(
            string bucketName,
            IList<IFormFile> filesToUpload)
        {
            if (filesToUpload is null)
            {
                return BadRequest("Request does not contain any files to be uploaded");
            }

            var uploadResponseFromRepository = await _fileRepository.UploadFilesAsync(bucketName, filesToUpload);

            if (uploadResponseFromRepository is null)
            {
                return BadRequest();
            }

            return Ok(uploadResponseFromRepository);
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> GetFiles(string bucketName)
        {
            var response = await _fileRepository.GetAllAsync(bucketName);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string fileName)
        {
            await _fileRepository.DeleteFileAsync(bucketName, fileName);

            return Ok();
        }

        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            var deleteResultFromRepo = await _fileRepository.DeleteFileAsync(bucketName, fileName);
                
            return Ok(deleteResultFromRepo);
        }

        [HttpPost]
        [Route("{bucketName}/addjsonobject")]
        public async Task<IActionResult> AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            await _fileRepository.AddJsonObjectAsync(bucketName, request);

            return Ok();
        }

        [HttpGet]
        [Route("{bucketName}/getjsonobject")]
        public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObject(string bucketName, string fileName)
        {
            var responseFromRepository = await _fileRepository.GetJsonObjectAsync(bucketName, fileName);

            return Ok(responseFromRepository);
        }
    }
}
