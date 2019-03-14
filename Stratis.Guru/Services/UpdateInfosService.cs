using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly TickerSettings _tickerSettings;

        public UpdateInfosService(IMemoryCache memoryCache, UpdateHub hub, IHubContext<UpdateHub> hubContext, IOptions<NakoApiSettings> nakoApiSettings, IOptions<TickerSettings> tickerSettings)
        {
            _memoryCache = memoryCache;
            _hub = hub;
            _hubContext = hubContext;
            _updateTimer = new System.Timers.Timer();
            _nakoApiSettings = nakoApiSettings.Value;
            _tickerSettings = tickerSettings.Value;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _updateTimer.AutoReset = false; // Make sure it only trigger once initially.

            _updateTimer.Elapsed += async (sender, args) =>
            {
                if (_updateTimer.AutoReset == false)
                {
                    _updateTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
                    _updateTimer.AutoReset = true;
                }

                var tickerApiClient = new RestClient(_tickerSettings.ApiUrl + "/simple/price?ids=stratis&vs_currencies=usd");
                var tickerApiRequest = new RestRequest(Method.GET);
                var tickerApiResponse = tickerApiClient.Execute(tickerApiRequest);
                dynamic ticker_response = JsonConvert.DeserializeObject(tickerApiResponse.Content);
                _memoryCache.Set("coin_price", (string)ticker_response.stratis.usd);

                var lastChangeApiClient = new RestClient(_tickerSettings.ApiUrl + "/coins/stratis?localization=false");
                var lastChangeApiRequest = new RestRequest(Method.GET);
                var lastChangeApiResponse = lastChangeApiClient.Execute(lastChangeApiRequest);
                dynamic last_change_response = JsonConvert.DeserializeObject(lastChangeApiResponse.Content);
                // Console.WriteLine(lastChangeApiResponse.Content);
                _memoryCache.Set("last_change", (string)last_change_response.market_data.price_change_percentage_24h);

                await _hubContext.Clients.All.SendAsync("UpdateTicker", cancellationToken);
                Console.WriteLine(DateTime.Now + " - Ticker Updated");

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