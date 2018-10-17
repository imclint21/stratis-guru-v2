using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Stratis.Guru.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
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

        [Route("generator")]
        public IActionResult Generator()
        {
            return View();
        }
    }
}