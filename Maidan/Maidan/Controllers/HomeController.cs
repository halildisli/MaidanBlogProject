using Maidan.Models;
using Maidan.Models.AuthenticationModels;
using Maidan.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Maidan.Controllers
{
    public class HomeController : Controller
    {
        private readonly MaidanDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(MaidanDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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

        [HttpGet]
        [Route("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(SignUpViewModel appUser)
        {

            var isUserExist = await _userManager.FindByEmailAsync(appUser.Email);
            if (isUserExist == null)
            {
                IdentityUser identityUser = new IdentityUser();
                identityUser.Email = appUser.Email;
                identityUser.UserName = appUser.Username;
                IdentityResult result = await _userManager.CreateAsync(identityUser, appUser.Password);
                if (result.Succeeded)
                {
                    var resultRole = await _userManager.AddToRoleAsync(identityUser, "USER");
                    if (resultRole.Succeeded)
                    {
                        TempData["Message"] = "Sign Up successfully!";
                        return RedirectToAction("SignIn", "Home");
                    }
                }
                else
                {
                    TempData["Message"] = "Sign Up unsuccessfully!";
                    return View();
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(SignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var hasUser = await _userManager.FindByEmailAsync(viewModel.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found!");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, viewModel.Password, viewModel.RememberMe, true);
            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Member");
            }

            ModelState.AddModelError(string.Empty, "Password in correct!");
            return View();

        }
    }
}