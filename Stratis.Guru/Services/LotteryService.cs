using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NBitcoin;
using Newtonsoft.Json;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class LotteryService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISettings _settings;
        private readonly IDraws _draws;
        private readonly DrawSettings _drawSettings;
        private readonly System.Timers.Timer _updateTimer;
        private DateTime _nextDraw;

        public LotteryService(IMemoryCache memoryCache, ISettings settings, IDraws draws, IOptions<DrawSettings> drawSettings)
        {
            _memoryCache = memoryCache;
            _settings = settings;
            _draws = draws;
            _drawSettings = drawSettings.Value;
            _updateTimer = new System.Timers.Timer();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            JackpotCounter();
            await InitLotteryAsync();
            await CalculateNextDrawAsync();

            _updateTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            _updateTimer.Enabled = true;
            _updateTimer.Elapsed += async (sender, args) =>
            {
                JackpotCounter();
                await CalculateNextDrawAsync();
            };
            _updateTimer.Start();
        }

        private void JackpotCounter()
        {
            Task.Run(() => 
            {
                var totalJackpot = 0.0;
                var pubkey = ExtPubKey.Parse(_drawSettings.PublicKey);
                for(int i=0; i<=_settings.GetIterator(); i++)
                {
                    var depositAddress = pubkey.Derive(0).Derive((uint)i).PubKey.GetAddress(Network.StratisMain).ToString();
                    var rc = new RestClient($"https://stratis.guru/api/address/{depositAddress}");
                    var rq = new RestRequest(Method.GET);
                    var response = rc.Execute(rq);
                    dynamic stratisAdressRequest = JsonConvert.DeserializeObject(response.Content);
                    totalJackpot += (double)stratisAdressRequest.balance;
                }
                _memoryCache.Set("Jackpot", totalJackpot);
            });
        }

        private async Task InitLotteryAsync() => await _settings.InitAsync();

        private async Task CalculateNextDrawAsync()
        {
            DateTime today = DateTime.UtcNow.Date;
            int daysUntilFriday = ((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7;
            _nextDraw = today.AddDays(daysUntilFriday);

            var nextDrawTimestamp = ((DateTimeOffset)_nextDraw).ToUnixTimeSeconds();

            await _draws.InitDrawAsync(nextDrawTimestamp);

            _memoryCache.Set("NextDraw", nextDrawTimestamp);
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