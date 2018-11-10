using FeedReader.Models;

namespace FeedReader.Services
{
    public interface IFeedService
    {
        Feed GetFeed(string url);
    }

    public class FeedService : IFeedService
    {
        public Feed GetFeed(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}