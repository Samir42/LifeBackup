using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace LifeBackup.Integration.Tests.Scenarios
{
    [Collection("api")]
    public class FilesControllerTests : ICollectionFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;


        public FilesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAWSService<IAmazonS3>(new AWSOptions
                    {
                        DefaultClientConfig =
                        {
                            ServiceURL = "http://localhost:9003"
                        },
                        Credentials = new BasicAWSCredentials("FAKE", "FAKE")
                    });
                });
            }).CreateClient();

            Task.Run(CreateBucket);
        }

        private async Task CreateBucket()
        {
            await _client.PostAsJsonAsync("api/bucket/create/babat-bucket", "babat-bucket");
        }

        [Fact]
        public async Task Upload_Files_Returns_Ok_When_Success()
        {
            var responseFromApiCall = await UploadFilesToS3Bucket();

            Assert.Equal(HttpStatusCode.OK, responseFromApiCall.StatusCode);
        }

        private async Task<HttpResponseMessage> UploadFilesToS3Bucket()
        {
            const string path = @"C:\S3Downloads\he_3.PNG";

            var file = File.Create(path);

            HttpContent content = new StreamContent(file);

            var formData = new MultipartFormDataContent
            {
                { content, "filesToUpload", "he_3.PNG" }
            };

            content.Dispose();
            formData.Dispose();

            return await _client.PostAsync("api/files/babat-bucket/add", formData);
        }
    }
}
