using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeedReader.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task SignUpSignIn()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = "/home/dashboard"
        });
    }
}