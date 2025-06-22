using System.Reflection;
using Coravel;
using FeedReader.Web.Data;
using FeedReader.Web.Services;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Extensions;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapStaticAssets();

app.UseHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
