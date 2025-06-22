namespace FeedReader.Web.ViewModels;

public class FeedItemResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Id { get; set; }
    public int FeedId { get; set; }
    public string[] Categories { get; set; } = [];
    public string? Author { get; set; }
    public string? Content { get; set; }
}