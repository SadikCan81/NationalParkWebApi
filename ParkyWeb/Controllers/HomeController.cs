using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModels;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Utility;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _npRepository;
        private readonly ITrailRepository _tRepository;
        private readonly IAccountRepository _accRepository;

        public HomeController(ILogger<HomeController> logger, INationalParkRepository npRepository,
                              ITrailRepository tRepository, IAccountRepository accRepository)
        {
            _logger = logger;
            _npRepository = npRepository;
            _tRepository = tRepository;
            _accRepository = accRepository;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwToken");

            IndexVM indexVM = new IndexVM()
            {
                NationalParkList = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, token),
                TrailList = await _tRepository.GetAllAsync(SD.TrailAPIPath, token)
            };

            return View(indexVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Authenticate authenticate)
        {
            var user = await _accRepository.LoginAsync(SD.UserAPIPath + "authenticate/", authenticate);

            if(user == null || user.Token == null)
            {
                ViewBag.Error = "User could not be found!";
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("JwToken", user.Token);
            TempData["alert"] = "Welcome " + user.UserName;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            var result = await _accRepository.RegisterAsync(SD.UserAPIPath + "register/", user);

            if (!result)
            {
                ViewBag.Error = "User could not be registered check your emails domain!";
                return View();
            }

            TempData["alert"] = "Successfully registered!";

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();

            HttpContext.Session.SetString("JwToken", "");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
