using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace zookeeper_leaderelection_client
{
    public class TestService : BackgroundService
    {
        private readonly IZooKeeperClient _client;
        private readonly ILogger<TestService> _logger;
        private readonly IServer server;

        public TestService(IZooKeeperClient client, ILogger<TestService> logger,IServer server)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.server = server;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //We'll do a loop where we check if we're the leader
            while (!stoppingToken.IsCancellationRequested)
                //Depending on the service, you can be leader for one, but not for an other
                if (await _client.IsLeader("testservice"))
                {
                    _logger.LogInformation("Look at me... I am the leader now!");
                    Console.WriteLine($"Look at me... I am the leader now!");
                    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses?.ToList();
                    addresses
                        .Select(P =>
                        {
                            Console.WriteLine(P);
                            Console.WriteLine("-------------");
                            return 0;
                        }).ToList();
                    //do stuff as the leader
                    return;
                }
        }
    }
}
