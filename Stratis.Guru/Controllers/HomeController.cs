using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using QRCoder;
using Stratis.Guru.Models;
using Stratis.Guru.Modules;

namespace Stratis.Guru.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAsk _ask;

        public HomeController(IMemoryCache memoryCache, IAsk ask)
        {
            _memoryCache = memoryCache;
            _ask = ask;
        }
        
        public IActionResult Index()
        {
            double displayPrice = 0;
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            dynamic coinmarketcap = JsonConvert.DeserializeObject(_memoryCache.Get("Coinmarketcap").ToString());
            var last24Change = coinmarketcap.data.quotes.USD.percent_change_24h / 100;
            
            if (rqf.RequestCulture.UICulture.Name.Equals("en-US"))
            {
                displayPrice = coinmarketcap.data.quotes.USD.price;
            }
            else
            {
                dynamic fixerApiResponse = JsonConvert.DeserializeObject(_memoryCache.Get("Fixer").ToString());
                var dollarRate = fixerApiResponse.rates.USD;
                try
                {
                    var regionInfo = new RegionInfo(rqf.RequestCulture.UICulture.Name.ToUpper());
                    var browserCurrencyRate = (double) ((JObject) fixerApiResponse.rates)[regionInfo.ISOCurrencySymbol];
                    displayPrice = 1 / (double) dollarRate * (double) coinmarketcap.data.quotes.USD.price * browserCurrencyRate;
                }
                catch
                {
                    // ignored
                }
            }
            
            return View(new Ticker
            {
                DisplayPrice = displayPrice,
                Last24Change = last24Change
            });
        }

        [Route("lottery")]
        public IActionResult Lottery()
        {
            ViewBag.NextDraw = long.Parse(_memoryCache.Get("NextDraw").ToString());
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        [Route("lottery/participate")]
        public IActionResult Participate()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Participate", new{id="5c1ef60f09f5754c34ba3010"});
            }
            ViewBag.NextDraw = long.Parse(_memoryCache.Get("NextDraw").ToString());
            ViewBag.Participate = true;
            return View("Lottery");
        }

        [HttpGet]
        [Route("lottery/participate/{id}")]
        public IActionResult Participate(string id)
        {
            ViewBag.NextDraw = long.Parse(_memoryCache.Get("NextDraw").ToString());
            ViewBag.Participate = true;
            return View("Lottery");
        }

        [HttpPost]
        [Route("lottery/check-payment/{address}")]
        public IActionResult CheckPayment(string address)
        {
            var pubkey = ExtPubKey.Parse("xpub6Bfq1wKQ64UUzW2HzQ6aZmoxkMYVq6DNifiTU1A1d9UEe16qSGnyqSAbFr42XAMXSXi3kcMXQJseXcgyTGQeDpBvHYt5m1HCH74Q52C4kkZ");
            var newAddress = pubkey.Derive(0).Derive(0).PubKey.GetAddress(Network.StratisMain);
            Console.WriteLine(newAddress);
            return Json(true);
        }

        [Route("about")]
        public IActionResult About()
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
            if (ModelState.IsValid)
            {
                _ask.NewVanity(vanity);
                ViewBag.Succeed = true;
            }
            return View();
        }

        [Route("generator")]
        public IActionResult Generator()
        {
            var stratisAddress = new Key();
            return View(new StratisAddressPayload
            {
                PrivateKey = stratisAddress.GetWif(Network.StratisMain).ToString(),
                PublicKey = stratisAddress.PubKey.GetAddress(Network.StratisMain).ToString()
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

        public IActionResult Documentation()
        {
            return Redirect("/documentation");
        }
    }
}