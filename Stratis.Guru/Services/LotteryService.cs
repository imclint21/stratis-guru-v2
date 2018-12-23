using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Stratis.Guru.Models;

namespace Stratis.Guru.Services
{
    public class LotteryService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private DateTime _nextDraw;

        public LotteryService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CalculateNextDrawAsync();
        }

        private async Task CalculateNextDrawAsync()
        {
            DateTime today = DateTime.Today;
            int daysUntilFriday = ((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7;
            _nextDraw = today.AddDays(daysUntilFriday);
            var nextDrawTimestamp = ((DateTimeOffset)_nextDraw).ToUnixTimeSeconds();

            #region use DI
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("stratis-guru");
            var collection = database.GetCollection<LotteryDraw>("draws");
            if(!collection.Find(x => x.DrawDate.Equals(nextDrawTimestamp)).Any())
            {
                await collection.InsertOneAsync(new LotteryDraw()
                {
                    DrawDate = nextDrawTimestamp,
                    Passed = false
                });
            }
            #endregion

            _memoryCache.Set("NextDraw", nextDrawTimestamp);
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