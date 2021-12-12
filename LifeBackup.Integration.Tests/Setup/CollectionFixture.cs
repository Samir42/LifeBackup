using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LifeBackup.Integration.Tests.Setup
{
    [CollectionDefinition("api")]
    public class CollectionFixture : ICollectionFixture<TestContext>
    {
        private readonly DockerClient _dockerClient;
        private const string ContainerImageUri = "localstack/localstack";
        private string _containerId;

        public CollectionFixture()
        {
            _dockerClient = new DockerClientConfiguration(new Uri(GetDockerUri()))
                .CreateClient();
        }

        public async Task InitializeAsync()
        {
            await PullImage();

            await StartContainer();
        }

        private async Task PullImage()
        {
            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = ContainerImageUri,
                Tag = "latest"
            },
            new AuthConfig(),
            new Progress<JSONMessage>());
        }

        public async Task StartContainer()
        {
            var responseFromDocker = await _dockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters
                {
                    Image = ContainerImageUri,
                    ExposedPorts = new Dictionary<string, EmptyStruct>
                    {
                        {
                            "9003", default
                        }
                    },
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            {
                                "9003", new List<PortBinding> { new PortBinding {HostPort= "9003" } }
                            }
                        }
                    },
                    Env = new List<string> { "SERVICES=s3:9003" }
                });

            _containerId = responseFromDocker.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }


        public async Task DisposeContainer()
        {
            if (_containerId != null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, null);
            }
        }

        private string GetDockerUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
                return "npipe://./pipe/docker_engine";


            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
                return "unix:/var/run/docker.sock";

            throw new Exception("Unable to determine what OS these tests running on");
        }

    }
}
