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
        private readonly CurrencyService _currencyService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly UpdateHub _hub;
        private readonly NakoSettings _nakoApiSettings;
        private readonly TickerSettings _tickerSettings;

        private readonly System.Timers.Timer _nakoTimer;
        private readonly System.Timers.Timer _tickerTimer;
        private readonly System.Timers.Timer _currencyTimer;

        public DataUpdateService(
            BlockIndexService blockIndexService,
            TickerService tickerService,
            CurrencyService currencyService,
            IMemoryCache memoryCache, 
            UpdateHub hub, 
            IHubContext<UpdateHub> hubContext, 
            IOptions<NakoSettings> nakoSettings, 
            IOptions<TickerSettings> tickerSettings)
        {
            _tickerService = tickerService;
            _blockIndexService = blockIndexService;
            _memoryCache = memoryCache;
            _hub = hub;
            _hubContext = hubContext;
            _nakoApiSettings = nakoSettings.Value;
            _tickerSettings = tickerSettings.Value;
            _currencyService = currencyService;

            _nakoTimer = new System.Timers.Timer();
            _tickerTimer = new System.Timers.Timer();
            _currencyTimer = new System.Timers.Timer();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartNakoTimer();

            StartTickerTimer(cancellationToken);

            StartCurrencyTimer();

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
                _memoryCache.Set("Ticker", _tickerService.GetTicker());
                
                await _hubContext.Clients.All.SendAsync("UpdateTicker", cancellationToken);
            };

            _tickerTimer.Start();
        }

        private void StartCurrencyTimer()
        {
            _currencyTimer.AutoReset = false; // Make sure it only trigger once initially.

            _currencyTimer.Elapsed += (sender, args) =>
            {
                if (_currencyTimer.AutoReset == false)
                {
                    // https://github.com/exchangeratesapi/exchangeratesapi
                    // The reference rates are usually updated around 16:00 CET on every working day, except on TARGET closing days. They are based on a regular daily concertation procedure between central banks across Europe, which normally takes place at 14:15 CET.

                    _currencyTimer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
                    _currencyTimer.AutoReset = true;
                }

                // Get ticker and cache it.
                _memoryCache.Set("Currency", _currencyService.GetRates("USD"));

                //await _hubContext.Clients.All.SendAsync("UpdateTicker", cancellationToken);
            };

            _currencyTimer.Start();
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
            
            _currencyTimer?.Stop();
            _currencyTimer?.Dispose();
        }
    }
}