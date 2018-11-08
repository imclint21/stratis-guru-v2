using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class FixerService : IHostedService, IDisposable
    {
        private FixerApiSettings _options;
        private IDistributedCache _distributedCache;
        private Timer _timer;

        public FixerService(IOptions<FixerApiSettings> options, IDistributedCache distributedCache)
        {
            _options = options.Value;
            _distributedCache = distributedCache;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var rs = new RestClient($"{_options.Endpoint}?access_key={_options.ApiKey}&format=1");
            var rq = new RestRequest(Method.GET);
            rs.ExecuteAsync(rq, delegate(IRestResponse response)
            {
                _distributedCache.SetString("Fixer", response.Content);
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