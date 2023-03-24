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
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;
        public MemberController(MaidanDbContext context, UserManager<Author> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Author> signInManager)
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
        public async Task<IActionResult> Index(int id)
        {
            var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var articlesForUser = _context.Articles.Where(a => a.AuthorId == identityUser.Id).ToList();
            return View(articlesForUser);
        }
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = _context.Articles.Find(id);
            var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var top3ArticlesForUser = _context.Articles.Where(a => a.AuthorId == identityUser.Id).OrderByDescending(a => a.ReleaseDate).Take(3).ToList();
            ViewBag.Top3Articles = top3ArticlesForUser;
            ViewBag.Author = identityUser;
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateArticle(ArticleViewModel createArticle)
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
        [HttpGet]
        public IActionResult UpdateArticle(int id)
        {
            Article foundedArticle = _context.Articles.Find(id);
            ArticleViewModel articleViewModel = new ArticleViewModel() { Id = foundedArticle.Id, Title = foundedArticle.Title, Content = foundedArticle.Content, Image = foundedArticle.Image };

            return View(articleViewModel);
        }
        [HttpPost]
        public IActionResult UpdateArticle(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                Article toBeUpdated = _context.Articles.Find(articleViewModel.Id);
                toBeUpdated.UpdateDate = DateTime.Now;
                toBeUpdated.Title = articleViewModel.Title;
                toBeUpdated.Content = articleViewModel.Content;
                toBeUpdated.Image = articleViewModel.Image;
                _context.Articles.Update(toBeUpdated);
                _context.SaveChanges();
                return RedirectToAction("Index", "Member");
            }
            return View();
        }
        [HttpGet]
        public IActionResult DeleteArticle(int id)
        {
            Article foundedArticle = _context.Articles.Find(id);
            ArticleViewModel articleViewModel = new ArticleViewModel() { Id = foundedArticle.Id, Title = foundedArticle.Title, Content = foundedArticle.Content, Image = foundedArticle.Image };

            return View(articleViewModel);
        }
        [HttpPost]
        public IActionResult DeleteArticle(ArticleViewModel articleViewModel)
        {

            Article toBeDeleted = _context.Articles.Find(articleViewModel.Id);
            _context.Articles.Remove(toBeDeleted);
            _context.SaveChanges();
            return RedirectToAction("Index", "Member");
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
