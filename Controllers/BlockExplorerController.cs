using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
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

        public BlockExplorerController(IOptions<NakoApiSettings> nakoApiSettings)
        {
            _nakoApiSettings = nakoApiSettings.Value;
        }
        
        public IActionResult Index()
        {
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
            return Content("oh yeay");
        }

        [Route("block/{block}")]
        public IActionResult Block(string block)
        {
            return View();
        }

        [Route("address/{address}")]
        public IActionResult Address(string address)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}address/{address}/transactions");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View(JsonConvert.DeserializeObject(endpointResponse.Content));
        }
        
        [Route("transaction/{transactionId}")]
        public IActionResult Transaction(string transactionId)
        {
            var endpointClient = new RestClient($"{_nakoApiSettings.Endpoint}transaction/{transactionId}");
            var enpointRequest = new RestRequest(Method.GET);
            var endpointResponse = endpointClient.Execute(enpointRequest);
            return View(JsonConvert.DeserializeObject(endpointResponse.Content));
        }
    }
}