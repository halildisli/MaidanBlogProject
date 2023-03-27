using AutoMapper;
using Maidan.Areas.Admin.ViewModels;
using Maidan.Models;
using Maidan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        public AdminController(UserManager<Author> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Author> signInManager, IMapper mapper, MaidanDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.AuthorsCount = _context.Authors.Count();
            ViewBag.ArticlesCount = _context.Articles.Count();
            ViewBag.TagsCount = _context.Tags.Count();
            var firstArticle = _context.Articles.OrderBy(a => a.ReleaseDate).FirstOrDefault();
            var serviceTime = (DateTime.Now - firstArticle.ReleaseDate).TotalDays;
            ViewBag.ServiceTime = serviceTime;
            return View();
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
