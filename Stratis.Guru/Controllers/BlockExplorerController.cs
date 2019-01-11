using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Stratis.Guru.Models;
using Stratis.Guru.Models.ApiModels;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;

namespace Stratis.Guru.Controllers
{
    [Route("block-explorer")]
    public class BlockExplorerController : Controller
    {
        private readonly NakoSettings _nakoApiSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly dynamic _stats;
        private readonly BlockIndexService _indexService;
        private readonly SetupSettings _setupSettings;
        private readonly FeaturesSettings _featuresSettings;

        public BlockExplorerController(IMemoryCache memoryCache,
            BlockIndexService indexService,
            IOptions<NakoSettings> nakoApiSettings,
            IOptions<SetupSettings> setupSettings,
            IOptions<FeaturesSettings> featuresSettings)
        {
            _memoryCache = memoryCache;
            _stats = JsonConvert.DeserializeObject(_memoryCache.Get("BlockchainStats").ToString());
            _indexService = indexService;

            _nakoApiSettings = nakoApiSettings.Value;
            _setupSettings = setupSettings.Value;
            _featuresSettings = featuresSettings.Value;
        }
        
        public IActionResult Index()
        {
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;

            var latestBlock = _indexService.GetLatestBlock();

            ViewBag.LatestBlock = latestBlock;
            ViewBag.BlockchainHeight = latestBlock.BlockIndex;

            var latestBlocks = new List<dynamic>();
            latestBlocks.Add(latestBlock);

            for (int i = (int)ViewBag.LatestBlock.BlockIndex-1; i >= (int)ViewBag.LatestBlock.BlockIndex-5; i--)
            {
                latestBlocks.Add(_indexService.GetBlockByHeight(i));
            }

            ViewBag.Blocks = latestBlocks;
            
            return View();

        }

        [Route("search")]
        public IActionResult Search(SearchBlockExplorer searchBlockExplorer)
        {
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;

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
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;
            ViewBag.BlockchainHeight = _stats.SyncBlockIndex;

            var result = (block.ToLower() == "latest") ? _indexService.GetLatestBlock() : _indexService.GetBlockByHeight(int.Parse(block));
            return View(result);
        }

        [Route("block/hash/{hash}")]
        public IActionResult BlockHash(string hash)
        {
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;
            ViewBag.BlockchainHeight = _stats.SyncBlockIndex;

            return View("Block", _indexService.GetBlockByHash(hash));
        }

        [Route("address/{address}")]
        public IActionResult Address(string address)
        {
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;
            ViewBag.BlockchainHeight = _stats.SyncBlockIndex;

            return View(_indexService.GetTransactionsByAddress(address));
        }
        
        [Route("transaction/{transactionId}")]
        public IActionResult Transaction(string transactionId)
        {
            ViewBag.Features = _featuresSettings;
            ViewBag.Setup = _setupSettings;
            ViewBag.BlockchainHeight = _stats.SyncBlockIndex;

            return View(_indexService.GetTransaction(transactionId));
        }
    }
}