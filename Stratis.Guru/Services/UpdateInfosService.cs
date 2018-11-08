using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RestSharp;
using Stratis.Guru.Hubs;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class UpdateInfosService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly UpdateHub _hub;
        private readonly System.Timers.Timer _updateTimer;
        private readonly NakoApiSettings _nakoApiSettings;

        public UpdateInfosService(IMemoryCache memoryCache, UpdateHub hub, IHubContext<UpdateHub> hubContext, IOptions<NakoApiSettings> nakoApiSettings)
        {
            _memoryCache = memoryCache;
            _hub = hub;
            _hubContext = hubContext;
            _updateTimer = new System.Timers.Timer();
            _nakoApiSettings = nakoApiSettings.Value;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _updateTimer.Interval = 10;
            _updateTimer.Enabled = true;
            _updateTimer.Elapsed += async (sender, args) =>
            {
                _updateTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
                var coinmarketCapApiClient = new RestClient("https://api.coinmarketcap.com/v2/ticker/1343/");
                var coinmarketCapApiRequest = new RestRequest(Method.GET);
                var coinmarketcapApi = coinmarketCapApiClient.Execute(coinmarketCapApiRequest);
                _memoryCache.Set("Coinmarketcap", coinmarketcapApi.Content);
                Console.WriteLine(DateTime.Now + " - Ticker Updated");
                await _hubContext.Clients.All.SendAsync("UpdateTicker", cancellationToken);

                var blockchainStatsClient = new RestClient($"{_nakoApiSettings.Endpoint}stats");
                var blockchainStatsRequest = new RestRequest(Method.GET);
                _memoryCache.Set("BlockchainStats", blockchainStatsClient.Execute(blockchainStatsRequest).Content);
            };
            _updateTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }
}