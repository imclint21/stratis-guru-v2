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
    public class DataUpdateService : IHostedService, IDisposable
    {
        private readonly BlockIndexService _blockIndexService;
        private readonly TickerService _tickerService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly UpdateHub _hub;
        private readonly FeaturesSettings _featuresSettings;

        private System.Timers.Timer _nakoTimer;
        private System.Timers.Timer _tickerTimer;

        public DataUpdateService(
            BlockIndexService blockIndexService,
            TickerService tickerService,
            IMemoryCache memoryCache, 
            UpdateHub hub, 
            IHubContext<UpdateHub> hubContext, 
            IOptions<NakoSettings> nakoSettings,
            IOptions<FeaturesSettings> featuresSettings)
        {
            _tickerService = tickerService;
            _blockIndexService = blockIndexService;
            _memoryCache = memoryCache;
            _hub = hub;
            _hubContext = hubContext;
            _featuresSettings = featuresSettings.Value;
            
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_featuresSettings.Explorer)
            {
                _nakoTimer = new System.Timers.Timer();
                StartNakoTimer();
            }

            if (_featuresSettings.Ticker)
            {
                _tickerTimer = new System.Timers.Timer();
                StartTickerTimer(cancellationToken);
            }

            return Task.CompletedTask;
        }

        private void StartNakoTimer()
        {
            _nakoTimer.AutoReset = false; // Make sure it only trigger once initially.

            _nakoTimer.Elapsed += (sender, args) =>
            {
                if (_nakoTimer.AutoReset == false)
                {
                    _nakoTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
                    _nakoTimer.AutoReset = true;
                }

                // Get statistics and cache it.
                _memoryCache.Set("BlockchainStats", _blockIndexService.GetStatistics());
            };

            _nakoTimer.Start();
        }
        

        private void StartTickerTimer(CancellationToken cancellationToken)
        {
            _tickerTimer.AutoReset = false; // Make sure it only trigger once initially.

            _tickerTimer.Elapsed += async (sender, args) =>
            {
                if (_tickerTimer.AutoReset == false)
                {
                    _tickerTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                    _tickerTimer.AutoReset = true;
                }

                // Get ticker and cache it.
                _memoryCache.Set("Ticker", _tickerService.DownloadTicker());
                _memoryCache.Set("Rates", _tickerService.DownloadRates());

                await _hubContext.Clients.All.SendAsync("UpdateTicker", cancellationToken);
            };

            _tickerTimer.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _nakoTimer?.Stop();
            _nakoTimer?.Dispose();

            _tickerTimer?.Stop();
            _tickerTimer?.Dispose();
        }
    }
}