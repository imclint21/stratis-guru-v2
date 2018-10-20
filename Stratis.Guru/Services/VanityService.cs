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
        private readonly IAsk _ask;

        public VanityService(IAsk ask)
        {
            _ask = ask;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_ask.GetVanities().Count > 0)
                    {
                        var getVanity = _ask.GetVanities().Dequeue();
                        
                        while (true)
                        {
                            var stratisKey = new Key();
                            var stratisAddress = stratisKey.GetWif(Network.StratisMain).PubKey.GetAddress(Network.StratisMain).ToString();
                
                            if (stratisAddress.ToLower().StartsWith(string.Concat("s", getVanity.Prefix)))
                            {
                                Console.WriteLine(stratisAddress + " - " + getVanity.Email);
                                break;
                            }
                        }
                    }
                    
                    Thread.Sleep((int) TimeSpan.FromSeconds(10).TotalMilliseconds);
                }
            }, cancellationToken);
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