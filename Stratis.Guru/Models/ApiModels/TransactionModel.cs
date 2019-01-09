using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class TransactionModel
    {
        public int Index { get; set; }

        public string Type { get; set; }

        public string TransactionHash { get; set; }

        public string SpendingTransactionHash { get; set; }

        public string PubScriptHex { get; set; }

        public string CoinBase { get; set; }

        public decimal Value { get; set; }

        public long BlockIndex { get; set; }

        public long Confirmations { get; set; }

        public int Time { get; set; }
    }
}
