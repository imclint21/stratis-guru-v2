using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using Stratis.Guru.Modules;

namespace Stratis.Guru.Services
{
    public class VanityService : IHostedService, IDisposable
    {
        private IMemoryCache _memoryCache;
        private IAsk _ask;

        public VanityService(IMemoryCache memoryCache, IAsk ask)
        {
            _memoryCache = memoryCache;
            _ask = ask;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            /*while (true)
            {
                var stratisKey = new Key();
                var stratisAddress = stratisKey.GetWif(Network.StratisMain).PubKey.GetAddress(Network.StratisMain).ToString();
                
                if (stratisAddress.ToLower().StartsWith("stra"))
                {
                    Console.WriteLine(stratisAddress);
                }
                
                //Thread.Sleep((int) TimeSpan.FromSeconds(10).TotalMilliseconds);
            }*/
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