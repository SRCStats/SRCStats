using Microsoft.AspNetCore.Mvc;
using SRCStats.Models;
using System.Diagnostics;

namespace SRCStats.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Disclaimer()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}