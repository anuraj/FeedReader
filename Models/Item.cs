using System;
using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required, Url]
        public string Url { get; set; }
        [Url]
        public string Image { get; set; }
        public string Content { get; set; }
        public long FeedId { get; set; }
        public Feed Feed  { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}