using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using FeedReader.Models;
using FeedReader.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace FeedReader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddDbContext<FeedReaderContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MYSQLCONNSTR_localdb")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFeedService, FeedService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
                {
                    var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
                    var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];
                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer
                    };
                    var uriString = $"{context.Request.Scheme}://{context.Request.Host}";
                    foreach (var cookie in context.Request.Cookies)
                    {
                        cookieContainer.Add(new Uri(uriString), new Cookie(cookie.Key, cookie.Value));
                    }
                    var jsonResult = string.Empty;
                    using (var client = new HttpClient(handler))
                    {
                        var res = await client.GetAsync($"{uriString}/.auth/me");
                        jsonResult = await res.Content.ReadAsStringAsync();
                    }
                    var obj = JArray.Parse(jsonResult);
                    var user_id = obj[0]["user_id"].Value<string>();
                    var claims = new List<Claim>();
                    foreach (var claim in obj[0]["user_claims"])
                    {
                        claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
                    }
                    var identity = new GenericIdentity(azureAppServicePrincipalNameHeader);
                    identity.AddClaims(claims);
                    context.User = new GenericPrincipal(identity, null);
                };

                await next.Invoke();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
