using Microsoft.Extensions.Options;
using Stratis.Guru.Settings;
using System;

namespace Stratis.Guru.Services
{
    public class CurrencyService : ServiceBase
    {
        private readonly CurrencySettings _currencySettings;

        public CurrencyService() : base(string.Empty)
        {

        }

        public CurrencyService(IOptions<CurrencySettings> currencySettings) : base(currencySettings.Value.ApiUrl)
        {
            _currencySettings = currencySettings.Value;
        }

        public double GetExchangePrice(double usd, string currency)
        {
            // Move the logic for this from the HomeController and in here.
            throw new NotImplementedException();
        }

        public string GetRates(string currency)
        {
            return Execute(GetRequest($"/latest?base={currency}"));
        }
    }
}
