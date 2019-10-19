using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Stratis.Guru.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.TagHelpers
{
    public class CoinTagHelper : TagHelper
    {
        private readonly SetupSettings setupSettings;

        [HtmlAttributeName("Positive")]
        public bool? Positive { get; set; }

        public CoinTagHelper(IOptions<SetupSettings> setupSettings)
        {
            this.setupSettings = setupSettings.Value;
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
                var values = (value / 100000000d).ToString("N8").Split('.');
                var html = $"<span class=\"coin-value-upper {cssExtra}\">{values[0]}</span><span class=\"coin-value-lower {cssExtra}\">.{values[1]}</span> <span class=\"coin-value-tag {cssExtra}\">{this.setupSettings.Coin}</span>";

                output.Content.SetHtmlContent(html);
            }
            else
            {
                output.Content.SetContent("");
            }
        }
    }
}
