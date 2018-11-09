using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class Feed
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}