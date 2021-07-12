using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;

namespace Transporter.IntegrationTests.Containers
{
    public class SqlServerContainer
    {
        private const int Port = 1433;
        private readonly TestcontainersContainer _container;
        private readonly Stream _outStream = new MemoryStream();
        private readonly Stream _errorStream = new MemoryStream();

        public SqlServerContainer()
        {
            var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");
            if (string.IsNullOrEmpty(dockerHost))
            {
                dockerHost = "unix:/var/run/docker.sock";
            }

            _container = new TestcontainersBuilder<TestcontainersContainer>()
                .WithDockerEndpoint(dockerHost)
                .WithImage("mcr.microsoft.com/mssql/server")
                .WithExposedPort(Port)
                .WithPortBinding(Port, Port)
                .WithEnvironment("ACCEPT_EULA", "y")
                .WithEnvironment("SA_PASSWORD", "_S1q2l3S4e5rver")
                .WithName($"sqlserver-test-container-{Guid.NewGuid()}")
                .WithOutputConsumer(Consume.RedirectStdoutAndStderrToStream(_outStream, _errorStream))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(Port))
                .Build();
        }

        public async Task StartAsync()
        {
            await _container.StartAsync();
        }

        public async Task StopAndDisposeAsync()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}