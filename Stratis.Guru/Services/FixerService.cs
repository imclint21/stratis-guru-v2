using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class FixerService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly FixerApiSettings _options;
        private readonly IMemoryCache _memoryCache;

        public FixerService(IOptions<FixerApiSettings> options, IMemoryCache memoryCache)
        {
            _options = options.Value;
            _memoryCache = memoryCache;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var rs = new RestClient($"{_options.Endpoint}?access_key={_options.ApiKey}&format=1");
            var rq = new RestRequest(Method.GET);
            rs.ExecuteAsync(rq, delegate(IRestResponse response)
            {
                _memoryCache.Set("Fixer", response.Content);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
