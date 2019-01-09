using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using Stratis.Guru.Models.ApiModels;
using Stratis.Guru.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stratis.Guru.Services
{
    public class BlockIndexService
    {
        private readonly NakoApiSettings _nakoApiSettings;
        private readonly RestClient _client;

        public BlockIndexService()
        {

        }

        public BlockIndexService(IOptions<NakoApiSettings> nakoApiSettings)
        {
            _nakoApiSettings = nakoApiSettings.Value;

            // RestClient is suppose to be able to be re-used. If not, we should move this into the Execute method.
            _client = new RestClient();
            _client.BaseUrl = new System.Uri(_nakoApiSettings.Endpoint);
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var response = _client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var blockIndexServiceException = new ApplicationException(message, response.ErrorException);
                throw blockIndexServiceException;
            }

            return response.Data;
        }

        private RestRequest GetRequest(string resource)
        {
            var request = new RestRequest();
            request.Method = Method.GET;
            request.Resource = resource;
            return request;
        }

        public BlockModel GetBlockByHeight(long blockHeight)
        {
            return Execute<BlockModel>(GetRequest($"query/block/index/{blockHeight}/transactions"));
        }

        public BlockModel GetBlockByHash(string blockHash)
        {
            return Execute<BlockModel>(GetRequest($"query/block/{blockHash}/transactions"));
        }

        public BlockModel GetLatestBlock()
        {
            return Execute<BlockModel>(GetRequest("query/block/Latest/transactions"));
        }

        public AddressModel GetTransactionsByAddress(string adddress)
        {
            return Execute<AddressModel>(GetRequest($"query/address/{adddress}/transactions"));
        }

        public TransactionDetailsModel GetTransaction(string transactionId)
        {
            return Execute<TransactionDetailsModel>(GetRequest($"query/transaction/{transactionId}"));
        }
    }
}
