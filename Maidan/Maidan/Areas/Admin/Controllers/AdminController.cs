using AutoMapper;
using Maidan.Areas.Admin.Models.ViewModels;
using Maidan.Models;
using Maidan.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Maidan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Author> _signInManager;
        private readonly IMapper _mapper;
        private readonly MaidanDbContext _context;
        public AdminController(UserManager<Author> userManager,RoleManager<IdentityRole> roleManager,SignInManager<Author> signInManager,IMapper mapper,MaidanDbContext context)
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
                if (_context.Tags.Where(t => t.Name == tag.Name).FirstOrDefault()==null)
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
            if (user!=null)
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
            var result= await _userManager.DeleteAsync(toBeDeleted);
            return RedirectToAction("ListUsers");
        }
    }
}
