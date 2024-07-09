using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TransporterService.Daemon
{
    [ExcludeFromCodeCoverage]
    public class HealthCheckDaemon(ILogger<HealthCheckDaemon> logger) : BackgroundService
    {
        private const int ListeningPort = 80;
        private static readonly byte[] Message = Encoding.ASCII.GetBytes("Pong");
        private readonly ILogger<HealthCheckDaemon> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Any, ListeningPort);

                server.Start();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var receiving = await Receive(server);

                    await Respond(stoppingToken, receiving.Stream);
                    
                    receiving.Client.Close();
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Health check listener error message: {e.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        private async Task<(TcpClient Client, Stream Stream)> Receive(TcpListener server)
        {
            var client = await server.AcceptTcpClientAsync();

            var stream = client.GetStream();

            return (client, stream);
        }

        private async Task Respond(CancellationToken stoppingToken, Stream stream)
        {
            var buffer = new byte[256];
            
            while ((await stream.ReadAsync(buffer, 0, buffer.Length, stoppingToken)) != 0)
            {
                await stream.WriteAsync(Message, 0, Message.Length, stoppingToken);
            }
        }
    }
}