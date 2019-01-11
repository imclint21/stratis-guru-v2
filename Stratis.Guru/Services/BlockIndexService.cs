using Microsoft.Extensions.Options;
using Stratis.Guru.Models.ApiModels;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Services
{
    public class BlockIndexService : ServiceBase
    {
        private readonly NakoSettings _nakoSettings;

        public BlockIndexService() : base(string.Empty)
        {

        }

        public BlockIndexService(IOptions<NakoSettings> nakoSettings) : base(nakoSettings.Value.ApiUrl)
        {
            _nakoSettings = nakoSettings.Value;
        }

        public BlockModel GetBlockByHeight(long blockHeight)
        {
            return Execute<BlockModel>(GetRequest($"/query/block/index/{blockHeight}/transactions"));
        }

        public BlockModel GetBlockByHash(string blockHash)
        {
            return Execute<BlockModel>(GetRequest($"/query/block/{blockHash}/transactions"));
        }

        public BlockModel GetLatestBlock()
        {
            return Execute<BlockModel>(GetRequest("/query/block/Latest/transactions"));
        }

        public AddressModel GetTransactionsByAddress(string adddress)
        {
            return Execute<AddressModel>(GetRequest($"/query/address/{adddress}/transactions"));
        }

        public TransactionDetailsModel GetTransaction(string transactionId)
        {
            return Execute<TransactionDetailsModel>(GetRequest($"/query/transaction/{transactionId}"));
        }

        public string GetStatistics()
        {
            var result = Execute(GetRequest("/stats"));
            return result;
        }
    }
}
