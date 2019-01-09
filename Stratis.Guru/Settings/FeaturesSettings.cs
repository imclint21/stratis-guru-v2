using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Settings
{
    public class FeaturesSettings
    {
        public bool Home { get; set; } = true;

        public bool Ticker { get; set; } = true;

        public bool Lottery { get; set; } = true;

        public bool Explorer { get; set; } = true;

        public bool Vanity { get; set; } = true;

        public bool Generator { get; set; } = true;

        public bool API { get; set; } = true;

        public bool About { get; set; } = true;

        public bool Footer { get; set; } = true;
    }
}
