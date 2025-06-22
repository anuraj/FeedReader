namespace FeedReader.Web.ViewModels;

public class FeedResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Id { get; set; }
    public int Count { get; set; }
    public List<FeedItemResponse> Items { get; set; } = [];
}