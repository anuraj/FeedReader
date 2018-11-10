using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class FeedUrl
    {
        [Required, Url]
        public string Url { get; set; }
    }
}