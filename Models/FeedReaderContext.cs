using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace FeedReader.Models
{
    public class FeedReaderContext : DbContext
    {
        public FeedReaderContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}