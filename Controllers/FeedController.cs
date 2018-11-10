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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FeedReader.Controllers
{
    public class FeedController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        private readonly IFeedService _feedService;
        public FeedController(IStorageService storageServicet, IUserService userService, IFeedService feedService)
        {
            _storageService = storageServicet;
            _userService = userService;
            _feedService = feedService;
        }

        public async Task<IActionResult> AddFeed(FeedUrl feedUrl)
        {
            if (ModelState.IsValid)
            {
                var response = await _feedService.GetFeedAsyc(feedUrl.Url, _userService.Id);
                response.UserId = _userService.Id;
                await _storageService.CreateFeed(response);
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}