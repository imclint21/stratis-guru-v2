using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stratis.Guru.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.TagHelpers
{
    public class CoinTagHelper : TagHelper
    {
        private readonly SetupSettings setupSettings;

        private readonly ILogger<CoinTagHelper> log;

        [HtmlAttributeName("Positive")]
        public bool? Positive { get; set; }

        public CoinTagHelper(IOptions<SetupSettings> setupSettings, ILogger<CoinTagHelper> log)
        {
            this.setupSettings = setupSettings.Value;
            this.log = log;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var input = output.GetChildContentAsync().Result.GetContent();
            long value;
            var success = long.TryParse(input, out value);

            var cssExtra = string.Empty;

            if (Positive.HasValue)
            {
                cssExtra = Positive.Value ? "coin-value-positive" : "coin-value-negative";
            }

            if (success)
            {
                try
                {
                    var values = (value / 100000000d).ToString("N8").Split(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
                    var html = $"<span class=\"coin-value-upper {cssExtra}\">{values[0]}</span><span class=\"coin-value-lower {cssExtra}\">{NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator}{values[1]}</span> <span class=\"coin-value-tag {cssExtra}\">{this.setupSettings.Coin}</span>";
                    output.Content.SetHtmlContent(html);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Failed to parse in CoinTagHelper. Input was {input} and parsed value was {value}.");
                    output.Content.SetContent(input);
                }
            }
            else
            {
                output.Content.SetContent(input);
            }
        }
    }
}
