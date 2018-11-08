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
using Stratis.Guru.Settings;

namespace Stratis.Guru.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly NakoApiSettings _nakoApiSettings;
        private readonly IMemoryCache _memoryCache;

        public ApiController(IMemoryCache memoryCache, IOptions<NakoApiSettings> nakoApiSettings)
        {
            _memoryCache = memoryCache;
            _nakoApiSettings = nakoApiSettings.Value;
        }
        
        [HttpGet]
        [Route("price")]
        public ActionResult<object> Price(bool notApi = false, double amount = 1)
        {
            try
            {
                dynamic coinmarketcap = JsonConvert.DeserializeObject(_memoryCache.Get("Coinmarketcap").ToString());
                if (notApi)
                {
                    var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            
                    double displayPrice = 0;
            
                    if (rqf.RequestCulture.UICulture.ThreeLetterISOLanguageName.Equals("eng"))
                    {
                        displayPrice = coinmarketcap.data.quotes.USD.price;
                    }
                    else
                    {
                        dynamic fixerApiResponse = JsonConvert.DeserializeObject(_memoryCache.Get("Fixer").ToString());
                        var dollarRate = fixerApiResponse.rates.USD;
                        try
                        {
                            var regionInfo = new RegionInfo(rqf.RequestCulture.UICulture.Name.ToUpper());
                            var browserCurrencyRate = (double) ((JObject) fixerApiResponse.rates)[regionInfo.ISOCurrencySymbol];
                            displayPrice = 1 / (double) dollarRate * (double) coinmarketcap.data.quotes.USD.price * browserCurrencyRate;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    
                    return new TickerApi
                    {
                        UsdPrice = (displayPrice * amount).ToString("C"),
                        Last24Change = (coinmarketcap.data.quotes.USD.percent_change_24h / 100).ToString("P2")
                    };
                }
                return new Ticker
                {
                    DisplayPrice = coinmarketcap.data.quotes.USD.price * amount,
                    Last24Change = coinmarketcap.data.quotes.USD.percent_change_24h / 100
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
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/address/{address}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }

        [HttpGet]
        [Route("transaction/{transaction}")]
        public ActionResult<object> Transaction(string transaction)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/transaction/{transaction}");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }

        [HttpGet]
        [Route("block/{block}")]
        public ActionResult<object> Block(string block)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/block/index/{block}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return JsonConvert.DeserializeObject(endpointResponse.Content);
        }
    }
}