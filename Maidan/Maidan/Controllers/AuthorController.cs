using Maidan.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maidan.Controllers
{
    public class AuthorController : Controller
    {
        private readonly MaidanDbContext _context;
        public AuthorController(MaidanDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var articlesOfAuthor=_context.Articles.Where(a => a.AuthorId == id).ToList();
            return View(articlesOfAuthor);
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index),author.Id);
        }
    }
}
