using Microsoft.AspNetCore.Mvc;

namespace Maidan.Controllers
{
    public class AuthorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
