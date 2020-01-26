using NBitcoin;

namespace Stratis.Guru.Models
{
    public class Ticker
    {
        public string Symbol { get; set; }

        public decimal Price { get; set; }

        public Money PriceBtc { get; set; }

        public double Last24Change { get; set; }
    }
}