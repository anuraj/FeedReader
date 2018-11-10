using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace FeedReader.Models
{
    public class FeedEntity : TableEntity
    {
        public FeedEntity()
        {
        }
        public FeedEntity(string userId)
        {
            Id = Guid.NewGuid();
            RowKey = Id.ToString();
            PartitionKey = userId;
        }

        [Key]
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required, MaxLength(255), Url]
        public string Url { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
    }
}