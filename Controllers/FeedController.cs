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
using Microsoft.ApplicationInsights;

namespace FeedReader.Controllers
{
    public class FeedController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        private readonly IFeedService _feedService;
        private readonly TelemetryClient _telemetryClient;
        public FeedController(IStorageService storageServicet, IUserService userService,
            IFeedService feedService, TelemetryClient telemetryClient)
        {
            _storageService = storageServicet;
            _userService = userService;
            _feedService = feedService;
            _telemetryClient = telemetryClient;
        }

        [HttpPost]
        public async Task<IActionResult> AddFeed(FeedUrl feedUrl)
        {
            if (ModelState.IsValid)
            {
                if (await _storageService.IsFeedAlreadyCreatedByMe(feedUrl.Url))
                {
                    return Conflict();
                }

                try
                {
                    var response = await _feedService.GetFeedAsyc(feedUrl.Url, _userService.Id);
                    response.FeedCount = response.Items.Count;
                    response.UserId = _userService.Id;
                    await _storageService.CreateFeed(response);
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex);
                    return StatusCode(500);
                }
                return Ok();
            }

            return BadRequest(ModelState);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetFeedsAsync()
        {
            if (_userService.IsAuthenticated)
            {
                var feeds = await _storageService.GetMyFeedsAsync();
                return Json(feeds);
            }

            return NotFound();
        }
    }
}