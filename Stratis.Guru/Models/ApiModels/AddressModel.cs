using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class AddressModel
    {
        public AddressModel()
        {
            Transactions = new List<TransactionModel>();
            UnconfirmedTransactions = new List<TransactionModel>();
        }

        public string CoinTag { get; set; }

        public string Address { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalReceived { get; set; }

        public decimal TotalSent { get; set; }

        public decimal UnconfirmedBalance { get; set; }

        public List<TransactionModel> Transactions { get; set; }

        public List<TransactionModel> UnconfirmedTransactions { get; set; }
    }
}
