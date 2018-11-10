using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class Feed
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required, MaxLength(255), Url]
        public string Url { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}