using Maidan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Maidan.Controllers
{
    public class HomeController : Controller
    {
        private readonly MaidanDbContext _context;

        public HomeController(MaidanDbContext context)
        {
            _context = context;
            if (_context.Articles.ToList().Count==0)
            {
                _context.Articles.Add(new Article { AuthorId = 1, Title = "Lorem", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 1, Title = "Lorem", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 1, Title = "Lorem", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 1, Title = "Lorem", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 1, Title = "Lorem", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 2, Title = "Ipsum", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 2, Title = "Ipsum", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 2, Title = "Ipsum", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 2, Title = "Ipsum", Content = "Lorem ipsum dolor" });
                _context.Articles.Add(new Article { AuthorId = 2, Title = "Ipsum", Content = "Lorem ipsum dolor" });
                _context.SaveChanges();
            }
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.ToList();

            return View(articles);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}