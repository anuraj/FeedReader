using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeedReader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FeedReader.Services
{
    public class StorageService : IStorageService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly IConfiguration _configuration;
        private readonly CloudTable _feedTable;
        private readonly CloudTable _feedItemsTable;
        private readonly IUserService _userService;
        public StorageService(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureStorageConnectionString"));
            _tableClient = _storageAccount.CreateCloudTableClient();
            _feedTable = _tableClient.GetTableReference("Feeds");
            _feedItemsTable = _tableClient.GetTableReference("FeedItems");
            _feedTable.CreateIfNotExistsAsync();
            _feedItemsTable.CreateIfNotExistsAsync();
        }
        public async Task CreateFeed(FeedEntity feed)
        {
            var insertFeed = TableOperation.Insert(feed);
            await _feedTable.ExecuteAsync(insertFeed);
        }

        public async Task<IEnumerable<FeedEntity>> GetMyFeedsAsync()
        {
            var query = new TableQuery<FeedEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _userService.Id));
            var tableResult = await _feedTable.ExecuteQuerySegmentedAsync(query, null);
            return tableResult.Results;
        }

        public async Task<bool> IsFeedAlreadyCreatedByMe(string url)
        {
            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _userService.Id);
            var urlFilter = TableQuery.GenerateFilterCondition("Url", QueryComparisons.Equal, url);
            var query = new TableQuery<DynamicTableEntity>();
            query.FilterString = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, urlFilter);
            var tableResult = await _feedTable.ExecuteQuerySegmentedAsync(query, null);
            return tableResult.Results.Count >= 1;
        }
    }
}