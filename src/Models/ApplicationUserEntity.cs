using System.ComponentModel.DataAnnotations;

namespace FeedReader.Web.Models;

public class ApplicationUserEntity
{
    public int Id { get; set; }
    [StringLength(50, ErrorMessage = "The UserId cannot exceed 50 characters.")]
    public string? UserId { get; set; }
    [StringLength(200, ErrorMessage = "The Name cannot exceed 100 characters.")]
    public string? Name { get; set; }
    [StringLength(200, ErrorMessage = "The Email cannot exceed 200 characters.")]
    public string? Email { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? LastLoggedInDate { get; set; }
    public ICollection<FeedEntity>? Feeds { get; set; } = new List<FeedEntity>();
}