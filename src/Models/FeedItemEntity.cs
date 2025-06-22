using System.ComponentModel.DataAnnotations;

namespace FeedReader.Web.Models;

public class FeedItemEntity
{
    public int Id { get; set; }
    [StringLength(500, ErrorMessage = "The title cannot exceed 500 characters.")]
    public string? Title { get; set; }
    [StringLength(2000, ErrorMessage = "The description cannot exceed 2000 characters.")]
    public string? Description { get; set; }
    [StringLength(500, ErrorMessage = "The link cannot exceed 500 characters.")]
    public string? Link { get; set; }
    public string? Content { get; set; }
    public DateTime PublishedDate { get; set; }
    [StringLength(200, ErrorMessage = "The author cannot exceed 200 characters.")]
    public string? Author { get; set; }
    [StringLength(500, ErrorMessage = "The image URL cannot exceed 500 characters.")]
    public string? ImageUrl { get; set; }
    public int FeedId { get; set; }
    public FeedEntity? Feed { get; set; }
    public string[]? Categories { get; set; }
}