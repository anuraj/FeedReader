namespace FeedReader.Web.Models;

public class FeedItemEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? Content { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    public int FeedId { get; set; }
    public FeedEntity? Feed { get; set; }
    public string[]? Categories { get; set; }
}