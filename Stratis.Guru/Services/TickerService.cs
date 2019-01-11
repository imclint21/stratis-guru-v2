using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Services
{
    public class TickerService: ServiceBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly NakoSettings _nakoApiSettings;
        private readonly TickerSettings _tickerSettings;


        public TickerService() : base(string.Empty)
        {

        }

        public TickerService(IMemoryCache memoryCache, IOptions<NakoSettings> nakoApiSettings, IOptions<TickerSettings> tickerSettings) : base(nakoApiSettings.Value.ApiUrl)
        {
            _memoryCache = memoryCache;
            _nakoApiSettings = nakoApiSettings.Value;
            _tickerSettings = tickerSettings.Value;
        }

        public string GetTicker()
        {
            var coinmarketCapApiClient = new RestClient(_tickerSettings.ApiUrl);
            var coinmarketCapApiRequest = new RestRequest(Method.GET);
            var coinmarketcapApi = coinmarketCapApiClient.Execute(coinmarketCapApiRequest);
            return coinmarketcapApi.Content;
        }

        public Ticker GetCachedTicker()
        {
            var ticker = new Ticker();

            var cachedResult = _memoryCache.Get<string>("Ticker"); // Responsibility of caching is put on UpdateInfosService.

            if (!string.IsNullOrWhiteSpace(cachedResult))
            {
                var json = JObject.Parse(cachedResult);

                double displayPrice = (double)json.SelectToken(_tickerSettings.PricePath);
                var last24Change = (double)json.SelectToken(_tickerSettings.PercentagePath) / 100;

                ticker.DisplayPrice = displayPrice;
                ticker.Last24Change = last24Change;
            }

            return ticker;
        }
    }
}
