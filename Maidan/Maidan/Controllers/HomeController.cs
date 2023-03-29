using AppUser.Management.Service.Models;
using AppUser.Management.Service.Services;
using AutoMapper;
using Maidan.Models;
using Maidan.OtherModels;
using Maidan.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;
        public HomeController(MaidanDbContext context, UserManager<Author> userManager, RoleManager<IdentityRole> roleManager,SignInManager<Author> signInManager,IWebHostEnvironment webHostEnvironment,IMapper mapper,IEmailService emailService,IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _emailService = emailService;
            _configuration = configuration;
        }
        public IActionResult SendMail(string name,string email)
        {

            var emailMessage = new MailMessage(new Dictionary<string, string>() { { name, email } }, "Maidan E-Mail Confirmation", "<h1>Welcome to ma<span style:\"color:#ffc017\">i<span>dan.</h1><hr/><p>Yazan: <b>Maidan Human Resources</b></p>");

            _emailService.SendEmail(emailMessage);


            return StatusCode(StatusCodes.Status200OK, new AppResponse() { Status = "Success", Message = "Email sent successfully!" });
        }
        public IActionResult Index()
        {
            List<Article> topTenArticles = _context.Articles.OrderByDescending(a => a.NumberOfReads).Take(10).ToList();
            List<Author> authorsOfTopTenArticles = new();
            foreach(Article item in topTenArticles)
            {
                Author author = _context.Authors.Where(a => a.Id == item.AuthorId).FirstOrDefault();
                authorsOfTopTenArticles.Add(author);
            }
            ViewBag.AuthorsOfTopTenArticles = authorsOfTopTenArticles;
            List<Article> lastSixArticles = _context.Articles.OrderByDescending(a => a.UpdateDate).Take(6).ToList();
            List<Author> authorsOfLastSixArticles = new();
            foreach (Article item in lastSixArticles)
            {
                Author author = _context.Authors.Where(a => a.Id == item.AuthorId).FirstOrDefault();
                authorsOfLastSixArticles.Add(author);
            }
            ViewBag.AuthorsOfLastSixArticles = authorsOfLastSixArticles;
            var authors = _context.Authors.ToList();
            ViewBag.Authors = authors;
            var articles = _context.Articles.ToList();
            //var tags= _context.Tags.ToList();
            //ViewBag.Tags = tags;
            return View(articles);
        }
        public IActionResult AllArticles()
        {
            List<Article> allArticles = _context.Articles.ToList();
            List<Author> authorsOfAllArticles = new();
            foreach (Article item in allArticles)
            {
                Author author = _context.Authors.Where(a => a.Id == item.AuthorId).FirstOrDefault();
                authorsOfAllArticles.Add(author);
            }
            ViewBag.AuthorsOfAllArticles = authorsOfAllArticles;
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
        public IActionResult Membership()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ListTags()
        {
            List<Tag> tags = _context.Tags.ToList();
            //foreach (var item in tags)
            //{
            //    item.Articles = _context.Articles.Where(a => a.Tags.Contains(item)).ToList();
            //}
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
                        #region Token Create and Verification Mail Send

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                        var emailConfirmationLink = Url.Action(nameof(ConfirmEmail), "Home", new { token, email = identityUser.Email }, Request.Scheme);
                        var emailVerificationMessage = new MailMessage(new Dictionary<string, string>() { { identityUser.UserName!, identityUser.Email! } },"Email Verification Link",$"<b>Your email verification link:</b>{emailConfirmationLink!}");
                        _emailService.SendEmail(emailVerificationMessage);


                        #endregion


                        TempData["Message"] = "Sign Up successfully! Please click on the activation link sent to your e-mail. ";
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

        /*
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
        */


        [HttpPost]
        public async Task<ActionResult> SignIn(SignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found!");
                return View();
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                #region JWT TOKEN YONTEMI
                /*
                    if (await _userManager.CheckPasswordAsync(user,viewModel.Password))
                    {

                        //Token'a payload eklemeleri yapılıyor.
                        var authClaims = new List<Claim> { new Claim(ClaimTypes.Name,user.UserName),new Claim
                            (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())};

                        var userRoles = await _userManager.GetRolesAsync(user);
                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }
                        var jwtToken = GetToken(authClaims);
                        return Ok(new {token=new JwtSecurityTokenHandler().WriteToken(jwtToken),expiration=jwtToken.ValidTo});



                    }
                */
                #endregion

                var signInResult = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, true);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username or password wrong!");
                    return View();
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "You cannot enter without e-mail verification!");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetArticle(int id)
        {
            var comments = _context.Comments.Where(c => c.ArticleId == id).ToList();
            ViewBag.Comments = comments;
            List<Author> authorsOfComments = new();
            foreach (var item in comments)
            {
                var author1 = _context.Authors.Where(a => a.Id == item.AuthorId).FirstOrDefault();
                if (author1 != null)
                {
                    authorsOfComments.Add(author1);
                }
            }
            ViewBag.AuthorsOfComments = authorsOfComments;
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

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(6),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        //TODO: (ACTIVATION CODE - 2. Madde)
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new AppResponse() { Status = "Onaylama başarılı!", Message = "Kullanıcının maili onaylandı!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new AppResponse() { Status = "Onaylama başarısız!", Message = "Kullanıcının token bilgisi yanlıştır!" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, new AppResponse() { Status = "Kullanıcı sistemde bulunmamaktadır!", Message = "Kullanıcı bulunamadı!" });
            }
        }

    }
}