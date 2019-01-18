using NBitcoin;

namespace Stratis.Guru.Models
{
    public class TickerApi
    {
        public string Symbol { get; set; }

        public string Price { get; set; }

        public string PriceBtc { get; set; }

        public string Last24Change { get; set; }
    }
}