using Coravel.Invocable;
using FeedReader.Web.Data;
using FeedReader.Web.Models;
using Microsoft.EntityFrameworkCore;
using FR = CodeHollow.FeedReader.FeedReader;
namespace FeedReader.Web.Services;

public class FeedFetcher : IInvocable, IInvocableWithPayload<FeedEntity>
{
    private readonly FeedReaderWebDbContext _feedReaderWebDbContext;
    private readonly ILogger<FeedFetcher> _logger;

    public FeedFetcher(FeedReaderWebDbContext feedReaderWebDbContext, ILogger<FeedFetcher> logger)
    {
        _feedReaderWebDbContext = feedReaderWebDbContext;
        _logger = logger;
    }

    public FeedEntity Payload { get; set; } = null!;

    public async Task Invoke()
    {
        var feed = await _feedReaderWebDbContext.Feeds
            .Include(f => f.Items)
            .FirstOrDefaultAsync(f => f.Id == Payload.Id);
        if (feed == null)
        {
            _logger.LogWarning("Feed with ID {FeedId} not found.", Payload.Id);
            return;
        }

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
        _logger.LogInformation("Fetched {ItemCount} items for feed with ID {FeedId}.", feed.Items.Count, feed.Id);
        _feedReaderWebDbContext.Feeds.Update(feed);
        await _feedReaderWebDbContext.SaveChangesAsync();
    }
}