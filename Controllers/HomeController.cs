using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NBitcoin;
using Newtonsoft.Json;
using QRCoder;
using Stratis.Bitcoin.Networks;
using Stratis.Guru.Models;
using Stratis.Guru.Modules;

namespace Stratis.Guru.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private IAsk _ask;

        public HomeController(IMemoryCache memoryCache, IAsk ask)
        {
            _memoryCache = memoryCache;
            _ask = ask;
        }
        
        public IActionResult Index()
        {
            dynamic coinmarketcap = JsonConvert.DeserializeObject(_memoryCache.Get("Coinmarketcap").ToString());
            return View(new Ticker
            {
                UsdPrice = coinmarketcap.data.quotes.USD.price
            });
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("block-explorer")]
        public IActionResult Explorer()
        {
            return View();
        }

        [Route("vanity")]
        public IActionResult Vanity()
        {
            return View();
        }

        [HttpPost]
        [Route("vanity")]
        public IActionResult Vanity(Vanity vanity)
        {
            return View();
        }

        [Route("generator")]
        public IActionResult Generator()
        {
            var stratisAddress = new Key();
            return View(new StratisAddressPayload
            {
                PrivateKey = stratisAddress.GetWif(Stratis.Bitcoin.Networks.StratisMain.Main).ToString(),
                PublicKey = stratisAddress.PubKey.GetAddress(Stratis.Bitcoin.Networks.StratisMain.Main).ToString()
            });
        }

        [Route("qr/{value}")]
        public IActionResult Qr(string value)
        {
            var memoryStream = new MemoryStream();
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.L);
            var qrCode = new QRCode(qrCodeData);
            qrCode.GetGraphic(20, Color.Black, Color.White, false).Save(memoryStream, ImageFormat.Png);
            return File(memoryStream.ToArray(), "image/png");
        }
    }
}