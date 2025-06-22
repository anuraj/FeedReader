using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FeedReader.Web.ViewModels;
using FeedReader.Web.Data;
using FeedReader.Web.Models;
using Microsoft.EntityFrameworkCore;
using FR = CodeHollow.FeedReader.FeedReader;
using X.PagedList.EF;
using Coravel.Queuing.Interfaces;
using FeedReader.Web.Services;
using SmartBreadcrumbs.Attributes;
namespace FeedReader.Web.Controllers;

[DefaultBreadcrumb]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FeedReaderWebDbContext _feedReaderWebDbContext;
    private readonly IQueue _queue;

    public HomeController(ILogger<HomeController> logger, FeedReaderWebDbContext feedReaderWebDbContext, IQueue queue)
    {
        _logger = logger;
        _feedReaderWebDbContext = feedReaderWebDbContext;
        _queue = queue;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Breadcrumb(FromAction = "Index", Title = "Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Breadcrumb(FromAction = "Index", Title = "Create")]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateFeedRequest feed, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            // Save the feed to the database
            var existingFeed = await _feedReaderWebDbContext.Feeds
                .FirstOrDefaultAsync(f => f.Url == feed.Url, cancellationToken);
            if (existingFeed == null)
            {
                var feedData = await FR.ReadAsync(feed.Url, cancellationToken: cancellationToken);
                if (feedData == null || !feedData.Items.Any())
                {
                    _logger.LogWarning("Feed with URL {Url} is not valid or does not contain any items.", feed.Url);
                    ModelState.AddModelError(string.Empty, "This feed URL is not valid or does not contain any items.");
                    return View(feed);
                }
                // If the feed is valid and does not exist, add it to the database
                _logger.LogInformation("Creating new feed with URL: {Url}", feed.Url);
                var FeedEntity = new FeedEntity
                {
                    Url = feed.Url,
                    Title = feedData.Title,
                    Description = feedData.Description,
                    ImageUrl = feedData.ImageUrl,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = null // Initial value, will be updated later
                };

                await _feedReaderWebDbContext.Feeds.AddAsync(FeedEntity, cancellationToken);
                await _feedReaderWebDbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Feed created successfully with URL: {Url}", feed.Url);

                _queue.QueueInvocableWithPayload<FeedFetcher, FeedEntity>(FeedEntity);

                return RedirectToAction("Dashboard");
            }
            else
            {
                _logger.LogWarning("Feed with URL {Url} already exists.", feed.Url);
                ModelState.AddModelError(string.Empty, "This feed URL already exists.");
                return View(feed);
            }
        }
        // If the model state is invalid, return the same view with the model to show validation errors
        _logger.LogWarning("Invalid feed data submitted.");
        ModelState.AddModelError(string.Empty, "Please correct the errors below.");
        return View(feed);
    }

    [HttpGet("validate-feed-url")]
    public async Task<IActionResult> ValidateFeedUrl(string url, CancellationToken cancellationToken)
    {
        //This method is used for remote validation of the feed URL
        if (string.IsNullOrWhiteSpace(url))
        {
            return Json("Please enter a valid URL.");
        }

        var existingFeed = await _feedReaderWebDbContext.Feeds.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Url == url, cancellationToken);
        if (existingFeed != null)
        {
            return Json("This feed URL already exists.");
        }

        try
        {
            var feed = await FR.ReadAsync(url, cancellationToken: cancellationToken);
            if (feed != null && feed.Items.Any())
            {
                return Json(true); // Valid feed URL
            }

            return Json("This feed URL does not contain any items.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating feed URL: {Url}", url);
            return Json("This feed URL is not valid.");
        }
    }

    [Breadcrumb(FromAction = "Index", Title = "Dashboard")]
    public async Task<IActionResult> Dashboard(int? page, CancellationToken cancellationToken)
    {
        if (page != null && page < 1)
        {
            page = 1;
        }
        var pageSize = 10; // Number of feeds per page
        var totalFeeds = await _feedReaderWebDbContext.Feeds.CountAsync(cancellationToken);
        var feeds = await _feedReaderWebDbContext.Feeds.AsNoTracking()
            .OrderByDescending(f => f.CreatedDate)
            .Select(f => new FeedResponse
            {
                Url = f.Url,
                Title = f.Title,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                CreatedDate = f.CreatedDate,
                Id = f.Id,
                Count = _feedReaderWebDbContext.FeedItems.Count(fi => fi.FeedId == f.Id)
            })
            .ToPagedListAsync(page ?? 1, pageSize, totalFeeds, cancellationToken);

        return View(feeds);
    }

    [Breadcrumb(FromAction = "Dashboard", Title = "Details")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var feed = await _feedReaderWebDbContext.Feeds
            .Include(f => f.Items)
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new FeedResponse
            {
                Url = f.Url,
                Title = f.Title,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                CreatedDate = f.CreatedDate,
                Id = f.Id,
                Items = f!.Items!
                    .Select(fi => new FeedItemResponse
                    {
                        Title = fi.Title,
                        Description = fi.Description,
                        ImageUrl = fi.ImageUrl,
                        Url = fi.Link,
                        CreatedDate = fi.PublishedDate,
                        Id = fi.Id,
                        FeedId = fi.FeedId,
                        Categories = fi!.Categories!,
                        Author = fi.Author,
                        Content = fi.Content
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (feed == null)
        {
            _logger.LogWarning("Feed with ID {Id} not found.", id);
            return NotFound();
        }

        return View(feed);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
