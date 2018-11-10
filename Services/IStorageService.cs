using System.Collections.Generic;
using System.Threading.Tasks;
using FeedReader.Models;

namespace FeedReader.Services
{
    public interface IStorageService
    {
        Task CreateFeed(FeedEntity feed);
        Task<IEnumerable<FeedEntity>> GetMyFeedsAsync();
    }
}