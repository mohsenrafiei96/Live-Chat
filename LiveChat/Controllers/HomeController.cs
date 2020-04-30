using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveChat.Models;
using Microsoft.AspNetCore.Authorization;

namespace LiveChat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        [Route("/Support")]
        public IActionResult SupportAgent() => View();
    }
}
