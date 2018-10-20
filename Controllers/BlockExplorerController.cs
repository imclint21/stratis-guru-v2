using System;
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
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
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
            return RedirectToAction("Index");
        }

        [Route("block/{block}")]
        public IActionResult Block(string block)
        {
            ViewBag.BlockchainHeight = _stats.syncBlockIndex;
            return View();
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