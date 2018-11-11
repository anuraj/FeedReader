using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using FeedReader.Models;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace FeedReader.Services
{
    public class FeedService : IFeedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public FeedService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<FeedEntity> GetFeedAsyc(string url, string userId)
        {
            var feed = new FeedEntity(userId);
            feed.Url = url;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var stream = await httpClient.GetStreamAsync(url);
                using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings() { Async = true }))
                {
                    var feedReader = new RssFeedReader(xmlReader);
                    while (await feedReader.Read())
                    {
                        switch (feedReader.ElementType)
                        {
                            case SyndicationElementType.Category:
                            case SyndicationElementType.Person:
                            case SyndicationElementType.Item:
                            case SyndicationElementType.Link:
                                break;
                            case SyndicationElementType.Image:
                                var image = await feedReader.ReadImage();
                                feed.Image = image.Url.ToString();
                                break;
                            default:
                                var content = await feedReader.ReadContent();
                                if (content.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
                                {
                                    feed.Title = content.Value;
                                }
                                if (content.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
                                {
                                    feed.Description = content.Value;
                                }
                                break;
                        }
                    }
                }
            }

            return feed;

        }
    }
}