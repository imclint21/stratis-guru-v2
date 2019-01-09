using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class BlockModel
    {
        public BlockModel()
        {
            Transactions = new List<string>();
        }

        public string CoinTag { get; set; }

        public string BlockHash { get; set; }

        public long BlockIndex { get; set; }

        public int BlockSize { get; set; }

        public long BlockTime { get; set; }

        public string NextBlockHash { get; set; }

        public string PreviousBlockHash { get; set; }

        public bool Synced { get; set; }

        public int TransactionCount { get; set; }

        public List<string> Transactions { get; set; }
    }
}
