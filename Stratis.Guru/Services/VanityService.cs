using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using Newtonsoft.Json;
using Stratis.Guru.Modules;

namespace Stratis.Guru.Services
{
    public class VanityService : IHostedService, IDisposable
    {
        private readonly IAsk _ask;
        private readonly IMemoryCache _memoryCache;

        public VanityService(IAsk ask, IMemoryCache memoryCache)
        {
            _ask = ask;
            _memoryCache = memoryCache;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_ask.GetVanities().Count > 0)
                    {
                        int iterator = 0;
                        var getVanity = _ask.GetVanities().Dequeue();

                        Parallel.For(0, int.MaxValue, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (i, loopState) =>
                        {
                            var stratisKey = new Key();
                            var stratisAddress = stratisKey.GetWif(Network.StratisMain).PubKey.GetAddress(Network.StratisMain).ToString();

                            if (stratisAddress.ToLower().StartsWith(getVanity.Prefix.ToLower()))
                            {
                                Console.WriteLine(stratisAddress + " - " + stratisKey.GetWif(Network.StratisMain));
                                _memoryCache.Set($"vanity[{getVanity.Prefix}]-pub", stratisAddress);  //JsonConvert.SerializeObject(new { privateKey = stratisKey.GetWif(Network.Main), publicKey = stratisAddress }));
                                _memoryCache.Set($"vanity[{getVanity.Prefix}]-pri", stratisKey.GetWif(Network.StratisMain).ToString());  //JsonConvert.SerializeObject(new { privateKey = stratisKey.GetWif(Network.Main), publicKey = stratisAddress }));
                                _memoryCache.Set($"vanity[{getVanity.Prefix}]-count", iterator.ToString());  //JsonConvert.SerializeObject(new { privateKey = stratisKey.GetWif(Network.Main), publicKey = stratisAddress }));
                                loopState.Break();
                            }
                            iterator++;
                        });

                        // ThreadPool.SetMaxThreads(10, 10);
                        // while(true)
                        // {
                        //     ThreadPool.QueueUserWorkItem((callback) =>
                        //     {
                        //         var stratisKey = new Key();
                        //         var stratisAddress = stratisKey.GetWif(Network.StratisMain).PubKey.GetAddress(Network.StratisMain).ToString();

                        //         if (stratisAddress.ToLower().StartsWith(string.Concat("s", prefix).ToLower()))
                        //         {
                        //             Console.WriteLine(stratisAddress + " - " + stratisKey.GetWif(Network.StratisMain));
                        //             foundedAddress = true;
                        //         }
                        //     }, null);
                        // }

                        // while (true)
                        // {
                        //     var stratisKey = new Key();
                        //     var stratisAddress = stratisKey.GetWif(Network.StratisMain).PubKey.GetAddress(Network.StratisMain).ToString();
                
                        //     if (stratisAddress.ToLower().StartsWith(string.Concat("s", getVanity.Prefix)))
                        //     {
                        //         Console.WriteLine(stratisAddress + " - " + getVanity.Email);
                        //         break;
                        //     }
                        // }
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