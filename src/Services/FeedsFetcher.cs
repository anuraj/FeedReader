using Coravel.Invocable;
using FeedReader.Web.Data;
using FeedReader.Web.Models;
using Microsoft.EntityFrameworkCore;
using FR = CodeHollow.FeedReader.FeedReader;
namespace FeedReader.Web.Services;

public class FeedsFetcher : IInvocable
{
    private readonly FeedReaderWebDbContext _feedReaderWebDbContext;
    private readonly ILogger<FeedsFetcher> _logger;

    public FeedsFetcher(FeedReaderWebDbContext feedReaderWebDbContext, ILogger<FeedsFetcher> logger)
    {
        _feedReaderWebDbContext = feedReaderWebDbContext;
        _logger = logger;
    }

    public async Task Invoke()
    {
        var feeds = await _feedReaderWebDbContext.Feeds
            .Include(f => f.Items)
            .Where(f => f.LastUpdatedDate == null || f.LastUpdatedDate < DateTime.UtcNow.AddHours(-1))
            .ToListAsync();

        foreach (var feed in feeds)
        {
            // Fetch and update the feed items
            var feedItems = await FR.ReadAsync(feed.Url, cancellationToken: default);
            feed.Items = feedItems?.Items
                .Select(item => new FeedItemEntity
                {
                    Title = item.Title,
                    Description = item.Description,
                    Link = item.Link,
                    PublishedDate = item.PublishingDate ?? DateTime.UtcNow,
                    Categories = item.Categories?.ToArray(),
                    Author = item.Author,
                    Content = item.Content,
                    FeedId = feed.Id
                }).ToList() ?? [];
            feed.LastUpdatedDate = DateTime.UtcNow;
        }

        await _feedReaderWebDbContext.SaveChangesAsync();
    }
}