using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class Status
    {
        public string CoinTag { get; set; }

        public string Progress { get; set; }

        public long TransactionsInPool { get; set; }

        public long SyncBlockIndex { get; set; }

        public long BlocksPerMinute { get; set; }

        public decimal AvgBlockPersistInSeconds { get; set; }

        public string Error { get; set; }

        public BlockChainInfo BlockChainInfo { get; set; }

        public NetworkInfo NetworkInfo { get; set; }
    }
}
