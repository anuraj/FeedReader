using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using FeedReader.Models;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace FeedReader.Services
{
    public interface IFeedService
    {
        Task<Feed> GetFeedAsyc(string url);
    }

    public class FeedService : IFeedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public FeedService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Feed> GetFeedAsyc(string url)
        {
            var feed = new Feed();
            feed.Items = new List<Item>();
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
                                break;
                            case SyndicationElementType.Image:
                                var image = await feedReader.ReadImage();
                                feed.Image = image.Url.ToString();
                                break;
                            case SyndicationElementType.Item:
                                var item = await feedReader.ReadItem();
                                var feedItem = new Item();
                                feedItem.Title = item.Title;
                                feedItem.Content = item.Description;
                                feedItem.Url = item.Links.FirstOrDefault()?.Uri.ToString();
                                feed.Items.Add(feedItem);
                                break;
                            case SyndicationElementType.Link:
                                var link = await feedReader.ReadLink();
                                feed.Url = link.Uri.ToString();
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