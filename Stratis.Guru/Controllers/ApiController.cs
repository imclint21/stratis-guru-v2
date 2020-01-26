using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ApiController> log;

        public ApiController(
            ILogger<ApiController> log,
            TickerService tickerService,
            CurrencyService currencyService,
            IMemoryCache memoryCache, 
            IOptions<NakoSettings> nakoApiSettings)
        {
            this.log = log;
            _tickerService = tickerService;
            _currencyService = currencyService;
            _memoryCache = memoryCache;
            _nakoApiSettings = nakoApiSettings.Value;
        }
        
        [HttpGet]
        [Route("price")]
        public ActionResult<object> Price(bool notApi = false, decimal amount = 1)
        {
            try
            {
                var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var regionInfo = _currencyService.GetRegionaInfo(rqf);
                var ticker = _tickerService.GetTicker(regionInfo.ISOCurrencySymbol);

                if (notApi)
                {
                    return new TickerApi
                    {
                        Symbol = ticker.Symbol,
                        PriceBtc = ticker.PriceBtc.ToString(),
                        Price = ticker.Price.ToString("C2"),
                        Last24Change = (ticker.Last24Change).ToString("P2")
                    };
                }

                return ticker;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get price from API.");
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
            try {
                var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/address/{address}/transactions");
                var endpointRequest = new RestRequest(Method.GET);
                endpointRequest.AddQueryParameter("api-version", "1.0");
                log.LogInformation($"Querying the indexer with URL: " + endpointRequest.ToString());
                var endpointResponse = endpointClient.Execute(endpointRequest);
                return JsonConvert.DeserializeObject(endpointResponse.Content);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get address.");
            }

            return null;
        }

        [HttpGet]
        [Route("transaction/{transaction}")]
        public ActionResult<object> Transaction(string transaction)
        {
            try
            {
                var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/transaction/{transaction}");
                var endpointRequest = new RestRequest(Method.GET);
                endpointRequest.AddQueryParameter("api-version", "1.0");
                log.LogInformation($"Querying the indexer with URL: " + endpointRequest.ToString());
                var endpointResponse = endpointClient.Execute(endpointRequest);
                return JsonConvert.DeserializeObject(endpointResponse.Content);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get transaction.");
            }

            return null;
        }

        [HttpGet]
        [Route("block/{block}")]
        public ActionResult<object> Block(string block)
        {
            try
            {
                var endpointClient = new RestClient($"{_nakoApiSettings.ApiUrl}query/block/index/{block}/transactions");
                var endpointRequest = new RestRequest(Method.GET);
                endpointRequest.AddQueryParameter("api-version", "1.0");
                log.LogInformation($"Querying the indexer with URL: " + endpointRequest.ToString());
                var endpointResponse = endpointClient.Execute(endpointRequest);
                return JsonConvert.DeserializeObject(endpointResponse.Content);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get block.");
            }

            return null;
        }
    }
}