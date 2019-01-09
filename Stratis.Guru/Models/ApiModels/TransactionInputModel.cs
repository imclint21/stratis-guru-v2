using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class TransactionInputModel
    {
        public int InputIndex { get; set; }

        public string InputAddress { get; set; }

        public string CoinBase { get; set; }

        public string InputTransactionId { get; set; }
    }
}
