using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Networks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly NakoSettings _nakoApiSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly TickerService _tickerService;
        private readonly CurrencyService _currencyService;

        public ApiController(
            TickerService tickerService,
            CurrencyService currencyService,
            IMemoryCache memoryCache, 
            IOptions<NakoSettings> nakoApiSettings)
        {
            _tickerService = tickerService;
            _currencyService = currencyService;
            _memoryCache = memoryCache;
            _nakoApiSettings = nakoApiSettings.Value;
        }
        
        [HttpGet]
        [Route("price")]
        public ActionResult<object> Price(bool notApi = false, double amount = 1)
        {
            try
            {
                var ticker = _tickerService.GetCachedTicker();

                if (notApi)
                {
                    var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            
                    double displayPrice = ticker.DisplayPrice;
            
                    if (!rqf.RequestCulture.UICulture.ThreeLetterISOLanguageName.Equals("eng"))
                    {
                        dynamic fixerApiResponse = JsonConvert.DeserializeObject(_currencyService.GetRates("USD"));
                        var dollarRate = fixerApiResponse.rates.USD;

                        try
                        {
                            var regionInfo = new RegionInfo(rqf.RequestCulture.UICulture.Name.ToUpper());
                            var browserCurrencyRate = (double) ((JObject) fixerApiResponse.rates)[regionInfo.ISOCurrencySymbol];
                            displayPrice = 1 / (double) dollarRate * (double)displayPrice * browserCurrencyRate;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    
                    return new TickerApi
                    {
                        UsdPrice = (displayPrice * amount).ToString("C"),
                        Last24Change = (ticker.Last24Change / 100).ToString("P2")
                    };
                }
                return new Ticker
                {
                    DisplayPrice = ticker.DisplayPrice * amount,
                    Last24Change = ticker.Last24Change / 100
                };
            }
            catch
            {
                //TODO: implement errors / logging
                return null;
            }
        }

        [HttpGet]
        [Route("create-address")]
        public ActionResult<object> CreateAddress()
        {
            var key = new Key();
            return new{PublicKey=key.PubKey.GetAddress(new StratisMain()).ToString(), PrivateKey=key.GetWif(new StratisMain()).ToString()};
        }

        [HttpGet]
        [Route("address/{address}")]
        public ActionResult<object> Address(string address)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/address/{address}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }

        [HttpGet]
        [Route("transaction/{transaction}")]
        public ActionResult<object> Transaction(string transaction)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/transaction/{transaction}");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }

        [HttpGet]
        [Route("block/{block}")]
        public ActionResult<object> Block(string block)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/block/index/{block}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }
    }
}