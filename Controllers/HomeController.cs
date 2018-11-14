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
using Microsoft.ApplicationInsights;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        private readonly TelemetryClient _telemetryClient;
        public HomeController(IStorageService storageService, IUserService userService, TelemetryClient telemetryClient)
        {
            _storageService = storageService;
            _userService = userService;
            _telemetryClient = telemetryClient;
        }

        public IActionResult Index()
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
