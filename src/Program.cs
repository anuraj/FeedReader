using System.Reflection;
using System.Security.Claims;
using Coravel;
using FeedReader.Web.Data;
using FeedReader.Web.Models;
using FeedReader.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var clientId = builder.Configuration["Authentication:Google:ClientId"];
var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.Events.OnTicketReceived = async ctx =>
        {
            var dbContext = ctx.HttpContext.RequestServices.GetRequiredService<FeedReaderWebDbContext>();
            if (ctx.Principal == null)
            {
                return;
            }
            var principal = ctx.Principal;
            var userId = principal!.FindFirstValue(ClaimTypes.NameIdentifier);
            var emailId = principal!.FindFirstValue(ClaimTypes.Email);
            var name = principal!.FindFirstValue(ClaimTypes.Name);
            
            var user = await dbContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserId == userId);
            
            if (user == null)
            {
                user = new ApplicationUserEntity
                {
                    UserId = userId,
                    Email = emailId,
                    Name = name,
                    CreatedDate = DateTime.UtcNow,
                    LastLoggedInDate = DateTime.UtcNow
                };
                dbContext.ApplicationUsers.Add(user);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                user.LastLoggedInDate = DateTime.UtcNow;
                dbContext.ApplicationUsers.Update(user);
                await dbContext.SaveChangesAsync();
            }

            // Replace the existing claims instead of adding new identity
            var identity = (ClaimsIdentity)ctx.Principal.Identity!;
            
            // Remove the Google NameIdentifier claim and add our database ID
            var googleNameIdentifierClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (googleNameIdentifierClaim != null)
            {
                identity.RemoveClaim(googleNameIdentifierClaim);
            }
            
            // Add our application-specific claims
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            
            // Ensure name and email claims are present
            if (identity.FindFirst(ClaimTypes.Name) == null && !string.IsNullOrEmpty(user.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            }
            if (identity.FindFirst(ClaimTypes.Email) == null && !string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            }
        };
    });


builder.Services.AddScoped<FeedFetcher>();
builder.Services.AddScoped<FeedsFetcher>();

builder.Services.AddQueue();
builder.Services.AddScheduler();

builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options =>
{
    options.TagName = "nav";
    options.TagClasses = "";
    options.OlClasses = "breadcrumb";
    options.LiClasses = "breadcrumb-item";
    options.ActiveLiClasses = "breadcrumb-item active";
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var connectionString = builder.Configuration.GetConnectionString("FeedReaderDbConnection") ??
    throw new InvalidOperationException("Connection string 'FeedReaderDbConnection' not found.");

builder.Services.AddDbContext<FeedReaderWebDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<FeedReaderWebDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.Services
    .UseScheduler(scheduler => scheduler.Schedule<FeedFetcher>()
    .Hourly()
    .PreventOverlapping(nameof(FeedFetcher)));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Fix middleware ordering - Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Remove duplicate authentication/authorization calls
app.UseHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
