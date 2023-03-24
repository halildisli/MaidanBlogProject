using Maidan.Models;
using Maidan.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using Maidan.Models.AuthenticationModels;
using Microsoft.AspNetCore.Authorization;

namespace Maidan.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly MaidanDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public MemberController(MaidanDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        //public IActionResult Index(int id)
        //{
        //    var articlesOfAuthor = _context.Articles.Where(a => a.AuthorId == id).ToList();
        //    return View(articlesOfAuthor);
        //}
        public IActionResult Index(int id)
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateArticle(CreateArticleViewModel createArticle)
        {
            if (ModelState.IsValid)
            {
                Article article = new Article();
                string userName = User.Identity.Name;

                var identityUser = await _userManager.FindByNameAsync(userName.ToUpper());


                article.AuthorId = identityUser.Id;
                article.Title = createArticle.Title;
                article.Content = createArticle.Content;
                article.Image = createArticle.Image;
                _context.Articles.Add(article);
                _context.SaveChanges();
            }
            return View();
        }


        //[HttpGet]
        //[Route("SignIn")]
        //public IActionResult SignIn()
        //{
        //    ClaimsPrincipal claimUser = HttpContext.User;
        //    if (claimUser.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Member");
        //    }
        //    return View();
        //}
        //[HttpPost]
        //[Route("SignIn")]
        //public async Task<IActionResult> SignIn(VMLogin loginModel)
        //{
        //    var foundedUser = _context.Users.Where(u => u.Email == loginModel.Email).FirstOrDefault();
        //    var hashedPassword = HashPassword(loginModel.Password);
        //    if (foundedUser != null && foundedUser.PasswordHash == hashedPassword)
        //    {
        //        List<Claim> claims = new List<Claim>()
        //        {
        //            new Claim(ClaimTypes.NameIdentifier,loginModel.Email),
        //            new Claim("Other Properties","Example Role")
        //        };
        //        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //        AuthenticationProperties properties = new AuthenticationProperties()
        //        {
        //            AllowRefresh = true,
        //            IsPersistent = loginModel.RememberMe

        //        };

        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

        //        TempData["ValidateMessage"] = "Login successfully!";
        //        return RedirectToAction("Index", "Member");
        //    }

        //    TempData["ValidateMessage"] = "User not found!!";
        //    return View();
        //}
        //public static string HashPassword(string password)
        //{
        //    byte[] salt;
        //    byte[] buffer2;
        //    if (password == null)
        //    {
        //        throw new ArgumentNullException("password");
        //    }
        //    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        //    {
        //        salt = bytes.Salt;
        //        buffer2 = bytes.GetBytes(0x20);
        //    }
        //    byte[] dst = new byte[0x31];
        //    Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        //    Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        //    return Convert.ToBase64String(dst);
        //}
    }
}
