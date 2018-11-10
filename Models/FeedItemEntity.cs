using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace FeedReader.Models
{
    public class FeedItemEntity : TableEntity
    {
        public FeedItemEntity(Guid feedId)
        {
            Id = Guid.NewGuid();
            RowKey = Id.ToString();
            FeedId = feedId;
            PartitionKey = FeedId.ToString();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required, Url]
        public string Url { get; set; }
        [Url]
        public string Image { get; set; }
        public string Content { get; set; }
        public Guid FeedId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}