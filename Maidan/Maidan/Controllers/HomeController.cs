using Maidan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Maidan.Controllers
{
    public class HomeController : Controller
    {
        private readonly MaidanDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,MaidanDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.OrderByDescending(a => a.ReleaseDate).ToList();

            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}