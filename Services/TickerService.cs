using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;

namespace Stratis.Guru.Services
{
    public class TickerService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        public TickerService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 10;
            updateTimer.Enabled = true;
            updateTimer.Elapsed += (sender, args) =>
            {
                var coinmarketCapApiClient = new RestClient("https://api.coinmarketcap.com/v2/ticker/1343/");
                var coinmarketCapApiRequest = new RestRequest(Method.GET);
                var coinmarketcapApi = coinmarketCapApiClient.Execute(coinmarketCapApiRequest);
                _memoryCache.Set("Coinmarketcap", coinmarketcapApi.Content);
                updateTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            };
            updateTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}