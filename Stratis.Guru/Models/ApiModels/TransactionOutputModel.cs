using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class TransactionOutputModel
    {
        public int Index { get; set; }

        public string Address { get; set; }

        public decimal Balance { get; set; }

        public string OutputType { get; set; }
    }
}
