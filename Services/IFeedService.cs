using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeedReader.Models;

namespace FeedReader.Services
{
    public interface IFeedService
    {
        Task<FeedEntity> GetFeedAsyc(string url, string userId);
    }
}