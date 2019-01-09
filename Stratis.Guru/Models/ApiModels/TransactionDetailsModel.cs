using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class TransactionDetailsModel
    {
        public TransactionDetailsModel()
        {
            Inputs = new List<TransactionInputModel>();
            Outputs = new List<TransactionOutputModel>();
        }

        public string CoinTag { get; set; }

        public string BlockHash { get; set; }

        public long BlockIndex { get; set; }

        [DataType(DataType.Date)]
        public DateTime Timestamp { get; set; }

        public string TransactionId { get; set; }

        public int Confirmations { get; set; }

        public List<TransactionInputModel> Inputs { get; set; }

        public List<TransactionOutputModel> Outputs { get; set; }
    }
}
