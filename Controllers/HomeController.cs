using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FeedReader.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using FeedReader.Services;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        public HomeController(IStorageService storageService, IUserService userService)
        {
            _storageService = storageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            if (_userService.IsAuthenticated)
            {
                var feeds = await _storageService.GetMyFeedsAsync();
                return View(feeds);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
