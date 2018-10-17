using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Stratis.Guru.Models;

namespace Stratis.Guru.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ApiController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        
        [HttpGet]
        [Route("price")]
        public ActionResult<object> Price(bool stringify, double amount = 1)
        {
            try
            {
                dynamic coinmarketcap = JsonConvert.DeserializeObject(_memoryCache.Get("Coinmarketcap").ToString());
                if (stringify)
                {
                    return new TickerApi
                    {
                        UsdPrice = (coinmarketcap.data.quotes.USD.price * amount).ToString("C"),
                        Last24Change = (coinmarketcap.data.quotes.USD.percent_change_24h / 100).ToString("P2")
                    };
                }
                return new Ticker
                {
                    UsdPrice = coinmarketcap.data.quotes.USD.price * amount,
                    Last24Change = coinmarketcap.data.quotes.USD.percent_change_24h / 100
                };
            }
            catch (Exception e)
            {
                //TODO: implement errors / logging
                return null;
            }
        }
    }
}