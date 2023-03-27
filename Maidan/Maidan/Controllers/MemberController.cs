using Maidan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Maidan.ViewModels;

namespace Maidan.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly MaidanDbContext _context;
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MemberController(MaidanDbContext context, UserManager<Author> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Author> signInManager, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        //public IActionResult Index(int id)
        //{
        //    var articlesOfAuthor = _context.Articles.Where(a => a.AuthorId == id).ToList();
        //    return View(articlesOfAuthor);
        //}
        public async Task<IActionResult> Index()
        {
            //List<Tag> tags=_context.Authors.Where()
            //var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            //var articlesForUser = _context.Articles.Where(a => a.AuthorId == identityUser.Id).ToList();
            var author = await _userManager.FindByNameAsync(User.Identity.Name);
            List<List<Article>> articlesList = new();
            List<Article> articles = new();
            ViewBag.Author = author.UserName;
            if (author != null)
            {
                var authorTags = _context.Tags.Where(a => a.Authors.Contains(author)).ToList();
                if (authorTags != null)
                {
                    foreach (var item in authorTags)
                    {
                        articlesList.Add(_context.Articles.Where(a => a.Tags.Contains(item)).ToList());
                    }
                    foreach (List<Article> item in articlesList)
                    {
                        foreach (Article article in item)
                        {
                            articles.Add(article);
                            
                        }
                    }
                    return View(articles);

                }
            }
            return BadRequest();
        }
        public async Task<IActionResult> MyArticles()
        {
            var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            //List<Tag> tags=_context.Authors.Where()
            var articlesForUser = _context.Articles.Where(a => a.AuthorId == identityUser.Id).ToList();
            return View(articlesForUser);
        }
        public async Task<IActionResult> AllArticles()
        {
            var author = await _userManager.FindByNameAsync(User.Identity.Name);
            if (author!=null)
            {
                ViewBag.Authorname = author.FirstName + " " + author.LastName;
                ViewBag.AuthorPhoto = author.Photo;
            }
            var allArticles = _context.Articles.ToList();
            return View(allArticles);
        }
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = _context.Articles.Find(id);
            var identityUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var top3ArticlesForUser = _context.Articles.Where(a => a.AuthorId == identityUser.Id).OrderByDescending(a => a.ReleaseDate).Take(3).ToList();
            ViewBag.Top3Articles = top3ArticlesForUser;
            ViewBag.Author = identityUser;
            article.NumberOfReads++;
            _context.Articles.Update(article);
            _context.SaveChanges();
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult CreateArticle()
        {
            var tagList = new List<SelectListItem>();
            foreach (Tag item in _context.Tags.ToList())
            {
                tagList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Tags = tagList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateArticle(ArticleViewModel createArticle, IFormFile photo, List<string> TagsList)
        {
            PhotoControl(photo);
            if (ModelState.IsValid)
            {
                Article article = new Article();
                string userName = User.Identity.Name;

                var identityUser = await _userManager.FindByNameAsync(userName.ToUpper());


                article.AuthorId = identityUser.Id;
                article.Title = createArticle.Title;
                article.Content = createArticle.Content;
                article.Image = AddPhotoArticle(photo);
                foreach (string tagIdStr in TagsList)
                {
                    int tagId = Convert.ToInt32(tagIdStr);
                    Tag tag = _context.Tags.Where(t => t.Id == tagId).FirstOrDefault();
                    if (tag != null)
                    {
                        Tag notExistTag = article.Tags.Where(t => t.Id == tag.Id).FirstOrDefault();
                        if (notExistTag == null)
                        {
                            article.Tags.Add(tag);
                        }
                    }
                }
                _context.Articles.Add(article);
                _context.SaveChanges();
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
                    ModelState.AddModelError("photo", "Extension must be .jpg, .jpeg, .png!");
                }
                else if (photo.Length > 1000 * 1000 * 1)//Bir MB'a karşılık geliyor.
                {
                    ModelState.AddModelError("photo", "Maximum file size 1 MB");
                }
            }
            else
            {
                ModelState.AddModelError("photo", "Photo is required!");
            }
        }
        private string? AddPhotoAuthor(IFormFile photo)
        {
            if (photo!=null)
            {
                string ext = Path.GetExtension(photo.FileName);
                string photoName = Guid.NewGuid() + ext;
                string photoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "authorImages", photoName);
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                return photoName;
            }
            return null;
        }
        private string? AddPhotoArticle(IFormFile photo)
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
        public IActionResult UpdateArticle(int id)
        {
            Article foundedArticle = _context.Articles.Find(id);
            ArticleViewModel articleViewModel = new ArticleViewModel() { Id = foundedArticle.Id, Title = foundedArticle.Title, Content = foundedArticle.Content };

            return View(articleViewModel);
        }
        [HttpPost]
        public IActionResult UpdateArticle(ArticleViewModel articleViewModel, IFormFile photo)
        {
            PhotoControl(photo);
            if (ModelState.IsValid)
            {
                Article toBeUpdated = _context.Articles.Find(articleViewModel.Id);
                toBeUpdated.UpdateDate = DateTime.Now;
                toBeUpdated.Title = articleViewModel.Title;
                toBeUpdated.Content = articleViewModel.Content;
                if (!string.IsNullOrEmpty(AddPhotoArticle(photo)))
                {
                    toBeUpdated.Image = AddPhotoArticle(photo);
                }
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
            ArticleViewModel articleViewModel = new ArticleViewModel() { Id = foundedArticle.Id, Title = foundedArticle.Title, Content = foundedArticle.Content };

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
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var tagList = new List<SelectListItem>();
            foreach (Tag item in _context.Tags.ToList())
            {
                tagList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Tags = tagList;
            var userName = User.Identity.Name;
            //var user = await _userManager.FindByNameAsync(userName);
            var user = _context.Authors.Where(a => a.UserName == userName).FirstOrDefault();
            MyProfileViewModel viewModel = _mapper.Map<MyProfileViewModel>(user);
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> MyProfile(MyProfileViewModel viewModel, List<string> TagsList, IFormFile? photo)
        {
            if (photo != null)
            {
                PhotoControl(photo);
            }
            if (ModelState.IsValid)
            {
                var author = await _userManager.FindByIdAsync(viewModel.Id);
                var userNameExist = await _userManager.FindByNameAsync(viewModel.UserName.ToUpper());
                if (userNameExist == null)
                {
                    ModelState.AddModelError(string.Empty, "This username already exists in the database. Please try another username.");
                    author.UserName = viewModel.UserName;
                }
                var emailExist = await _userManager.FindByEmailAsync(viewModel.Email.ToUpper());
                if (emailExist == null)
                {
                    ModelState.AddModelError(string.Empty, "This e-mail already exists in the database. Please try another e-mail.");
                    author.Email = viewModel.Email;
                }
                author.PhoneNumber = viewModel.PhoneNumber;
                author.FirstName = viewModel.FirstName;
                author.LastName = viewModel.LastName;
                author.LastName = viewModel.LastName;
                author.Bio = viewModel.Bio;
                author.SubDomain = viewModel.SubDomain;
                author.GithubUrl = viewModel.GithubUrl;
                author.LinkedInUrl = viewModel.GithubUrl;
                author.GithubUrl = viewModel.LinkedInUrl;
                author.TwitterUrl = viewModel.TwitterUrl;
                author.InstagramUrl = viewModel.InstagramUrl;
                author.WebsiteUrl = viewModel.WebsiteUrl;
                if (!string.IsNullOrEmpty(AddPhotoAuthor(photo)))
                {
                    author.Photo = AddPhotoAuthor(photo);
                }
                foreach (string tagIdStr in TagsList)
                {
                    int tagId = Convert.ToInt32(tagIdStr);
                    Tag tag = _context.Tags.Where(t => t.Id == tagId).FirstOrDefault();
                    if (tag != null)
                    {
                        Tag notExistTag = author.Tags.Where(t => t.Id == tag.Id).FirstOrDefault();
                        if (notExistTag == null)
                        {
                            author.Tags.Add(tag);
                        }
                    }
                }
                var result = await _userManager.UpdateAsync(author);
                TempData["Message"] = "Update has been successfully!";
                return RedirectToAction("MyProfile", "Member");
            }
            TempData["Message"] = "Update failed!";
            return RedirectToAction("MyProfile", "Member");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            Author author = await _userManager.FindByIdAsync(id);
            ChangePasswordViewModel viewModel = new();
            viewModel.Id = author.Id;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel passwordViewModel)
        {
            if (ModelState.IsValid)
            {
                var toBeUpdated = await _userManager.FindByIdAsync(passwordViewModel.Id);
                if (toBeUpdated != null)
                {
                    var result = await _userManager.ChangePasswordAsync(toBeUpdated, passwordViewModel.CurrentPassword, passwordViewModel.NewPassword);
                    if (result.Succeeded)
                    {
                        TempData["PasswordChange"] = "Your password has been successfully changed!";
                        return RedirectToAction("MyProfile", "Member");
                    }
                    TempData["PasswordChange"] = "An error occurred while changing your password. Try again later!";
                    return RedirectToAction("MyProfile", "Member");
                }
            }
            TempData["PasswordChange"] = "An error occurred while changing your password. Try again later!";
            return RedirectToAction("MyProfile", "Member");
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
