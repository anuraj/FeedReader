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
        private readonly FeedReaderContext _feedReaderContext;
        private readonly IUserService _userService;
        public HomeController(FeedReaderContext feedReaderContext, IUserService userService)
        {
            _feedReaderContext = feedReaderContext;
            _userService = userService;
        }

        public IActionResult Index()
        {
            if (_userService.IsAuthenticated)
            {
                var feeds = _feedReaderContext.Feeds.Where(f => f.UserId == _userService.Id)?.AsEnumerable();
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
