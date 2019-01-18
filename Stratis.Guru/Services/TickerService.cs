using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NBitcoin;
using Newtonsoft.Json.Linq;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class TickerService: ServiceBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly NakoSettings _nakoApiSettings;
        private readonly TickerSettings _tickerSettings;
        private readonly CurrencySettings _currencySettings;

        public TickerService() : base(string.Empty)
        {

        }

        public TickerService(IMemoryCache memoryCache, 
            IOptions<NakoSettings> nakoApiSettings,
            IOptions<TickerSettings> tickerSettings,
            IOptions<CurrencySettings> currencySettings
            ) : base(nakoApiSettings.Value.ApiUrl)
        {
            _memoryCache = memoryCache;
            _nakoApiSettings = nakoApiSettings.Value;
            _tickerSettings = tickerSettings.Value;
            _currencySettings = currencySettings.Value;
        }

        public string DownloadTicker()
        {
            var client = new RestClient(_tickerSettings.ApiUrl);
            var request = new RestRequest(Method.GET);
            var result = client.Execute(request);
            return result.Content;
        }

        public string DownloadRates()
        {
            var client = new RestClient(_currencySettings.ApiUrl);
            var request = new RestRequest(Method.GET);
            var result = client.Execute(request);
            return result.Content;
        }

        public Ticker GetTicker(string currency)
        {
            if (!_currencySettings.AutoConvert)
            {
                currency = "USD";
            }

            var ticker = new Ticker();

            var cachedTickerResult = _memoryCache.Get<string>("Ticker"); // Responsibility of caching is put on UpdateInfosService.

            if (!string.IsNullOrWhiteSpace(cachedTickerResult))
            {
                var json = JObject.Parse(cachedTickerResult);

                var last24Change = (double)json.SelectToken(_tickerSettings.PercentagePath) / 100;

                if (_tickerSettings.IsBitcoinPrice)
                {
                    Money price;
                    Money.TryParse(json.SelectToken(_tickerSettings.PricePath).ToString(), out price);
                    ticker.PriceBtc = price;
                }
                else
                {
                    var price = (decimal)json.SelectToken(_tickerSettings.PricePath);
                    ticker.Price = price; // Set the USD price, might be replaced with local currency price.
                }
                
                ticker.Last24Change = last24Change;
            }

            var cachedRateResult = _memoryCache.Get<string>("Rates"); // Responsibility of caching is put on UpdateInfosService.

            if (!string.IsNullOrWhiteSpace(cachedRateResult))
            {
                var json = JObject.Parse(cachedRateResult);
                JToken rateCurrency = json.SelectToken("$.data[?(@.symbol == '" + currency + "')]");
                JToken rateBtc = json.SelectToken("$.data[?(@.symbol == 'BTC')]");

                ticker.Symbol = (string)rateCurrency.SelectToken("currencySymbol");
                var currencyUsdRate = (decimal)rateCurrency.SelectToken("rateUsd");
                var btcUsdRate = (decimal)rateBtc.SelectToken("rateUsd");

                if (_tickerSettings.IsBitcoinPrice)
                {
                    // First calculate the price of the BTC in USD.
                    var usdPrice = ticker.PriceBtc.ToDecimal(MoneyUnit.BTC) * btcUsdRate;

                    // Calculate the price of the USD in the local currency, if different than USD.
                    ticker.Price = (1 / currencyUsdRate) * usdPrice;
                }
                else
                {
                    // Take the bitcoin price and multiply with USD price.
                    var btcPrice = (1 / btcUsdRate) * ticker.Price;
                    ticker.PriceBtc = Money.FromUnit(btcPrice, MoneyUnit.BTC);

                    // Get the local currency price.
                    ticker.Price = (1 / currencyUsdRate) * ticker.Price;
                }
            }

            return ticker;
        }
    }
}
