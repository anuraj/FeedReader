using System.ComponentModel.DataAnnotations;

namespace FeedReader.Web.Models;

public class FeedEntity
{
    public int Id { get; set; }
    [StringLength(500, ErrorMessage = "The URL cannot exceed 500 characters.")]
    public string? Title { get; set; }
    [StringLength(1000, ErrorMessage = "The description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
    [StringLength(500, ErrorMessage = "The URL cannot exceed 500 characters.")]
    public string? Url { get; set; }
    [StringLength(500, ErrorMessage = "The image URL cannot exceed 500 characters.")]
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public ICollection<FeedItemEntity>? Items { get; set; } = new List<FeedItemEntity>();
    public ApplicationUserEntity? CreatedBy { get; set; }
}