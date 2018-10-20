using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Controllers
{
    [Route("block-explorer")]
    public class BlockExplorerController : Controller
    {
        private readonly NakoApiSettings _nakoApiSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly dynamic _stats;

        public BlockExplorerController(IMemoryCache memoryCache, IOptions<NakoApiSettings> nakoApiSettings)
        {
            _nakoApiSettings = nakoApiSettings.Value;
            _memoryCache = memoryCache;
            _stats = JsonConvert.DeserializeObject(_memoryCache.Get("BlockchainStats").ToString());
        }
        
        public IActionResult Index()
        {
            var latestBlockClient = new RestClient($"{_nakoApiSettings.Endpoint}query/block/Latest/transactions");
            var latestBlockRequest = new RestRequest(Method.GET);
            var latestBlockResponse = latestBlockClient.Execute(latestBlockRequest);
            ViewBag.LatestBlock = JsonConvert.DeserializeObject(latestBlockResponse.Content);
            
            ViewBag.BlockchainHeight = ViewBag.LatestBlock.blockIndex;

            var x = new List<dynamic>();

            for (int i = (int)ViewBag.LatestBlock.blockIndex; i >= (int)ViewBag.LatestBlock.blockIndex-5; i--)
            {
                var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/block/index/{i}/transactions");
                var enpointRequest = new RestRequest(Method.GET);
                var endpointResponse = endpointClient.Execute(enpointRequest);
                x.Add(JsonConvert.DeserializeObject(endpointResponse.Content));
            }

            ViewBag.Blocks = x;
            
            return View();

        }

        [Route("search")]
        public IActionResult Search(SearchBlockExplorer searchBlockExplorer)
        {
            if (searchBlockExplorer.Query.Length == 34)
            {
                return RedirectToAction("Address", new {address = searchBlockExplorer.Query});
            }
            else if (searchBlockExplorer.Query.Length == 64)
            {
                return RedirectToAction("Transaction", new {transactionId = searchBlockExplorer.Query});
            }
            else if (searchBlockExplorer.Query.Length <= 8)
            {
                return RedirectToAction("Block", new {block = searchBlockExplorer.Query});
            }
            return RedirectToAction("Index");
        }

        [Route("block/{block}")]
        public IActionResult Block(string block)
        {
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/block/index/{block}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View(JsonConvert.DeserializeObject(endpointResponse.Content));
        }

        [Route("block/hash/{hash}")]
        public IActionResult BlockHash(string hash)
        {
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/block/{hash}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View("Block", JsonConvert.DeserializeObject(endpointResponse.Content));
        }

        [Route("address/{address}")]
        public IActionResult Address(string address)
        {
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/address/{address}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View(JsonConvert.DeserializeObject(endpointResponse.Content));
        }
        
        [Route("transaction/{transactionId}")]
        public IActionResult Transaction(string transactionId)
        {
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}query/transaction/{transactionId}");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View(JsonConvert.DeserializeObject(endpointResponse.Content));
        }
    }
}