using Maidan.Areas.Admin.Models.ViewModels;
using Maidan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Maidan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;
        public HomeController(UserManager<Author> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Author> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "The data you entered is invalid.");
                return View();
            }
            var adminName = viewModel.Username.ToUpper();
            var admin = await _userManager.FindByNameAsync(adminName);
            if (admin!=null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(admin, viewModel.Password, false, true);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View();
        }
    }
}
