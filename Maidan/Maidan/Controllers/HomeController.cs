using AutoMapper;
using Maidan.Models;
using Maidan.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Maidan.Controllers
{
    public class HomeController : Controller
    {
        private readonly MaidanDbContext _context;
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IMapper _mapper;
        public HomeController(MaidanDbContext context, UserManager<Author> userManager, RoleManager<IdentityRole> roleManager,SignInManager<Author> signInManager,IWebHostEnvironment webHostEnvironment,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var authors = _context.Authors.ToList();
            ViewBag.Authors = authors;
            var articles = _context.Articles.ToList();
            //var tags= _context.Tags.ToList();
            //ViewBag.Tags = tags;
            return View(articles);
        }
        public IActionResult AllArticles()
        {
            var authors = _context.Authors.ToList();
            ViewBag.Authors = authors;
            var articles = _context.Articles.ToList();
            return View(articles);
        }
        public async Task<IActionResult> GetAuthor(string userName)
        {
            var author = await _userManager.FindByNameAsync(userName);
            if (author!=null)
            {
                if (author.Photo != null)
                {
                    ViewBag.AuthorPhoto = author.Photo;
                }
                else
                {
                    ViewBag.AuthorPhoto = "";
                }
                MyProfileViewModel viewModel = _mapper.Map<MyProfileViewModel>(author);
                return View(viewModel);
            }
            return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> AuthorArticles(string userName)
        {
            var author = await _userManager.FindByNameAsync(userName);
            var articles = _context.Articles.Where(a => a.AuthorId == author.Id).ToList();
            return View(articles);
        }

        public IActionResult AboutUs()
        {
            ViewBag.AuthorsCount = _context.Authors.Count();
            ViewBag.ArticlesCount = _context.Articles.Count();
            ViewBag.TagsCount = _context.Tags.Count();
            var firstArticle = _context.Articles.OrderBy(a => a.ReleaseDate).FirstOrDefault();
            var serviceTime = (DateTime.Now - firstArticle.ReleaseDate).TotalDays;
            ViewBag.ServiceTime = serviceTime;
            return View();
        }
        public IActionResult Membership()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ListTags()
        {
            List<Tag> tags = _context.Tags.ToList();
            return View(tags);
        }

        [HttpGet]
        public IActionResult TagArticles(int id)
        {
            var tag = _context.Tags.Where(t => t.Id == id).FirstOrDefault();
            ViewBag.Tag = tag.Name;
            if (tag!=null)
            {
                var articles=_context.Articles.Where(a => a.Tags.Contains(tag)).ToList();
                if (articles!=null)
                {
                    return View(articles);
                }
            }
            return RedirectToAction(nameof(ListTags));
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
				Author identityUser = new Author();
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

        private void PhotoControl(IFormFile photo)
        {
            string[] photoExtensions = { ".jpg", ".png", ".jpeg" };
            if (photo != null)
            {
                string ext = Path.GetExtension(photo.FileName);
                if (!photoExtensions.Contains(ext))
                {
                    ModelState.AddModelError("formFile", "Extension must be .jpg, .jpeg, .png!");
                }
                else if (photo.Length > 1000 * 1000 * 1)//Bir MB'a karşılık geliyor.
                {
                    ModelState.AddModelError("formFile", "Maximum file size 1 MB");
                }
            }
            else
            {
                ModelState.AddModelError("formFile", "Photo is required!");
            }
        }
        private string? AddPhoto(IFormFile photo)
        {
            string ext = Path.GetExtension(photo.FileName);
            string photoName = Guid.NewGuid() + ext;
            string photoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "articleImages", photoName);
            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }
            return photoName;
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
        [HttpGet]
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = _context.Articles.Find(id);
            var author = await _userManager.FindByIdAsync(article.AuthorId);
            var top3ArticlesForAuthor = _context.Articles.Where(a => a.AuthorId == author.Id).OrderByDescending(a => a.NumberOfReads).Take(3).ToList();
            ViewBag.Top3Articles = top3ArticlesForAuthor;
            article.NumberOfReads++;
            _context.Articles.Update(article);
            _context.SaveChanges();
            ViewBag.Author = _context.Authors.Where(a => a.Id == article.AuthorId).FirstOrDefault();
            return View(article);
        }

    }
}