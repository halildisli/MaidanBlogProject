using AutoMapper;
using Maidan.Areas.Admin.Models.ViewModels;
using Maidan.Models;
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
    }
}
