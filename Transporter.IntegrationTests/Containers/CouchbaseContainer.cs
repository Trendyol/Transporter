using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using DotNet.Testcontainers.Containers.WaitStrategies;

namespace Transporter.IntegrationTests.Containers
{
    public class CouchbaseContainer
    {
        private readonly CouchbaseTestcontainer _container;

        public CouchbaseContainer()
        {
            var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");
            if (string.IsNullOrEmpty(dockerHost))
            {
                dockerHost = "unix:/var/run/docker.sock";
            }

            _container = new TestcontainersBuilder<CouchbaseTestcontainer>()
                .WithDockerEndpoint(dockerHost)
                .WithDatabase(new CouchbaseTestcontainerConfiguration
                {
                    Username = "Administrator",
                    Password = "password",
                    BucketName = "test-source-bucket"
                })
                .WithImage("docker.io/mustafaonuraydin/couchbase-testcontainer:6.5.1")
                .WithName($"CouchbaseContainer-{Guid.NewGuid()}")
                .WithExposedPort(8091)
                .WithExposedPort(8093)
                .WithExposedPort(11210)
                .WithPortBinding(8091, 8091)
                .WithPortBinding(8093, 8093)
                .WithPortBinding(11210, 11210)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8091))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8093))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(11210))
                .Build();
        }

        public async Task CreateBucket(string bucket)
        {
            await _container.CreateBucket(bucket);
        }

        public async Task StartAsync()
        {
            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}