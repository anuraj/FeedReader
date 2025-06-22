using FeedReader.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedReader.Web.Data;

public class FeedReaderWebDbContext : DbContext
{
    public FeedReaderWebDbContext(DbContextOptions options) : base(options)
    {
    }

    protected FeedReaderWebDbContext()
    {
    }

    public DbSet<FeedEntity> Feeds { get; set; }
    public DbSet<FeedItemEntity> FeedItems { get; set; }
}