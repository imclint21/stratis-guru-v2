using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stratis.Guru.Settings;
using System.Collections.Generic;
using System.Globalization;

namespace Stratis.Guru.Services
{
    public class CurrencyService : ServiceBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CurrencySettings _currencySettings;

        public static readonly Dictionary<string, string> CustomCultures = new Dictionary<string, string>() { { "EN", "EN-US" }, { "NO", "NB-NO" }, { "NB", "NB-NO" } };

        public CurrencyService() : base(string.Empty)
        {
            
        }

        public CurrencyService(
            IMemoryCache memoryCache,
            IOptions<CurrencySettings> currencySettings) : base(currencySettings.Value.ApiUrl)
        {
            _memoryCache = memoryCache;
            _currencySettings = currencySettings.Value;
        }

        public RegionInfo GetRegionaInfo(IRequestCultureFeature rqf)
        {
            // Whenever the culture has been specified in the URL, write it to a cookie. This ensures that the culture selection is
            // available in the REST API/Web Socket call and updates, and when the user visits the website next time.
            //if (!string.IsNullOrWhiteSpace(this.Request.Query["culture"]))
            //{
            //    CookieRequestCultureProvider.MakeCookieValue(rqf.RequestCulture);
            //}

            RegionInfo regionInfo = new RegionInfo("EN-US");

            if (!rqf.RequestCulture.UICulture.Name.Equals("en-US") && !rqf.RequestCulture.UICulture.Name.Equals("en"))
            {
                try
                {

                    string culture = rqf.RequestCulture.UICulture.Name.ToUpper();

                    if (CustomCultures.ContainsKey(culture))
                    {
                        culture = CustomCultures[culture];
                    }

                    regionInfo = new RegionInfo(culture);
                }
                catch { }
            }

            return regionInfo;
        }


        public double GetExchangePrice(double usd, string currency)
        {
            if (currency != "USD")
            {
                try
                {
                    dynamic fixerApiResponse = JsonConvert.DeserializeObject(_memoryCache.Get("Currency").ToString());

                    dynamic dollarRate = fixerApiResponse.rates.USD;

                    double browserCurrencyRate = (double)((JObject)fixerApiResponse.rates)[currency];

                    double displayPrice = 1 / (double)dollarRate * usd * browserCurrencyRate;

                    return displayPrice;
                }
                catch { }
            }

            return usd;
        }

        public string GetRates(string currency)
        {
            return Execute(GetRequest($"/latest?base={currency}"));
        }
    }
}
