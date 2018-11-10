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
    public class FeedController : Controller
    {
        private readonly FeedReaderContext _feedReaderContext;
        private readonly IUserService _userService;
        private readonly IFeedService _feedService;
        public FeedController(FeedReaderContext feedReaderContext, IUserService userService, IFeedService feedService)
        {
            _feedReaderContext = feedReaderContext;
            _userService = userService;
            _feedService = feedService;
        }

        public async Task<IActionResult> AddFeed(Feed feed)
        {
            if (ModelState.IsValid)
            {
                var response = await _feedService.GetFeedAsyc(feed.Url);
                response.UserId = _userService.Id;
                _feedReaderContext.Feeds.Add(response);
                var affectedRows = await _feedReaderContext.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}