using AutoMapper;
using Maidan.Areas.Admin.ViewModels;
using Maidan.Models;
using Maidan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Diagnostics;

namespace Maidan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;
        private readonly IMapper _mapper;
        private readonly MaidanDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(UserManager<Author> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Author> signInManager, IMapper mapper, MaidanDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.AuthorsCount = _context.Authors.Count();
            ViewBag.ArticlesCount = _context.Articles.Count();
            ViewBag.TagsCount = _context.Tags.Count();
            var firstArticle = _context.Articles.OrderBy(a => a.ReleaseDate).FirstOrDefault();
            double serviceTime = 0;
            try
            {
                serviceTime = (DateTime.Now - firstArticle.ReleaseDate).TotalDays;
            }
            catch (Exception)
            {

                serviceTime = 6.0;
            }
            ViewBag.ServiceTime = serviceTime;
            return View();
        }
        public IActionResult ListArticles()
        {
            List<Author> authors = _context.Authors.ToList();
            foreach(Article item in _context.Articles.ToList())
            {
                var tempAuthor = _context.Authors.Where(a => a.Id == item.AuthorId).FirstOrDefault();
                authors.Add(tempAuthor);
            }
            ViewBag.Authors = authors;
            List<ArticleViewModel> viewModels = _mapper.Map<List<ArticleViewModel>>(_context.Articles.ToList());
            return View(viewModels);
        }
        [HttpGet]
        public IActionResult EditArticle(int id)
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
            Article foundedArticle = _context.Articles.Find(id);
            //ArticleViewModel articleViewModel = new ArticleViewModel() { Id = foundedArticle.Id, Title = foundedArticle.Title, Content = foundedArticle.Content };
            ArticleViewModel viewModel = _mapper.Map<ArticleViewModel>(foundedArticle);
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditArticle(ArticleViewModel viewModel, IFormFile photo, List<string> TagsList)
        {
            if (photo != null)
            {
                PhotoControl(photo);

            }
            if (ModelState.IsValid || (viewModel.Title != null && viewModel.Content != null))
            {
                Article toBeUpdated = _context.Articles.Find(viewModel.Id);
                toBeUpdated.UpdateDate = DateTime.Now;
                toBeUpdated.Title = viewModel.Title;
                toBeUpdated.Content = viewModel.Content;
                if (!string.IsNullOrEmpty(AddPhotoArticle(photo)))
                {
                    toBeUpdated.Image = AddPhotoArticle(photo);
                }
                else
                {
                    toBeUpdated.Image = "default-article-image.jpg";
                }
                toBeUpdated.Tags.Clear();
                foreach (string tagIdStr in TagsList)
                {
                    int tagId = Convert.ToInt32(tagIdStr);
                    Tag tag = _context.Tags.Where(t => t.Id == tagId).FirstOrDefault();
                    if (tag != null)
                    {
                        Tag notExistTag = toBeUpdated.Tags.Where(t => t.Id == tag.Id).FirstOrDefault();
                        if (notExistTag == null)
                        {
                            toBeUpdated.Tags.Add(tag);
                        }
                    }
                }
                try
                {
                    _context.Articles.Update(toBeUpdated);
                    await _context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    toBeUpdated.Tags.Clear();
                    _context.Articles.Update(toBeUpdated);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ListArticles", "Admin",new {area="Admin"});
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
            if (photo != null)
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
            if (photo != null)
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
            return null;
        }



        [HttpGet]
        public IActionResult DeleteArticle(int id)
        {
            Article article = _context.Articles.Find(id);
            _context.Articles.Remove(article);
            _context.SaveChanges();
            return RedirectToAction(nameof(ListArticles));
        }


        [HttpGet]
        public IActionResult AddTag()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddTag(TagViewModel tagModel)
        {
            if (ModelState.IsValid)
            {
                Tag tag = _mapper.Map<Tag>(tagModel);
                if (_context.Tags.Where(t => t.Name == tag.Name).FirstOrDefault() == null)
                {
                    _context.Tags.Add(tag);
                    _context.SaveChanges();
                    TempData["Message"] = "Tag added successfully!";
                    return View();
                }
                TempData["Message"] = "Tag added failed!";
                return View();
            }
            TempData["Message"] = "You entered incorrect information!Please try again.";
            return View();
        }
        [HttpGet]
        public IActionResult EditTag(int id)
        {
            Tag tag = _context.Tags.Find(id);
            return View(tag);
        }
        [HttpPost]
        public IActionResult EditTag(Tag tagUpdated)
        {
            if (ModelState.IsValid)
            {
                _context.Tags.Update(tagUpdated);
                _context.SaveChanges();
                TempData["Message"] = "Tag editing succussfully!";
                return RedirectToAction("ListTags");
            }
            return View();
        }
        public IActionResult DeleteTag(int id)
        {
            _context.Tags.Remove(_context.Tags.Find(id));
            _context.SaveChanges();
            return RedirectToAction("ListTags");
        }
        [HttpGet]
        public IActionResult ListTags()
        {
            var tags = _context.Tags.ToList();
            List<TagViewModel> tagModels = _mapper.Map<List<TagViewModel>>(tags);
            return View(tagModels);
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _context.Authors.ToList();
            List<UserViewModel> userModels = _mapper.Map<List<UserViewModel>>(users);
            return View(userModels);
        }
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(AdminViewModel viewModel)
        {
            Author author = _mapper.Map<Author>(viewModel);
            var foundedUser = await _userManager.FindByEmailAsync(viewModel.Email);
            if (foundedUser==null)
            {
                IdentityResult result = await _userManager.CreateAsync(author,viewModel.Password);
                if (result.Succeeded)
                {
                    var resultRole = await _userManager.AddToRoleAsync(author, "ADMIN");
                    if (resultRole.Succeeded)
                    {
                        TempData["Message"] = "Admin created successfully!";
                        return RedirectToAction("ListUsers", "Admin");
                    }
                }
                else
                {
                    TempData["Message"] = "Admin create process failed!";
                    return RedirectToAction("ListUsers", "Admin");
                }
            }
            TempData["Message"] = "Admin create process failed!";
            return RedirectToAction("ListUsers", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            UserViewModel userModel = _mapper.Map<UserViewModel>(user);
            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel userModel)
        {
            var user = await _userManager.FindByIdAsync(userModel.Id);
            if (user != null)
            {
                user.UserName = userModel.UserName;  //???????
                user.Email = userModel.Email;
                user.PhoneNumber = userModel.PhoneNumber;
                user.FirstName = userModel.LastName;
                user.Photo = userModel.Photo;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
            }
            return View(userModel);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            UserViewModel userModel = _mapper.Map<UserViewModel>(user);
            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserViewModel incomeModel)
        {
            var toBeDeleted = await _userManager.FindByIdAsync(incomeModel.Id);
            var result = await _userManager.DeleteAsync(toBeDeleted);
            return RedirectToAction("ListUsers");
        }
        [HttpGet]
        public async Task<IActionResult> SignOutAdmin()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new {Area="Admin"});
        }
    }
}
