using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConnectionMultiplexer _redis;

        public HomeController(ILogger<HomeController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public IActionResult Index()
        {
            var db = _redis.GetDatabase();

            var list = db.ListRange("images");
            List<string> images = list.Select(x => x.ToString()).ToList();

            return View(images);
        }
    }
}
