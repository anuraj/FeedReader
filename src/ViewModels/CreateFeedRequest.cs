using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FeedReader.Web.ViewModels;

public class CreateFeedRequest
{
    [Url(ErrorMessage = "Please enter a valid URL.")]
    [Required(ErrorMessage = "URL is required.")]
    [Display(Name = "Feed URL")]
    [Remote("ValidateFeedUrl", "Home", ErrorMessage = "This feed URL is not valid or already exists.")]
    public string? Url { get; set; }

}
