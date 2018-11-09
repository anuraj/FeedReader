using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer
                    };
                    string uriString = $"{context.Request.Scheme}://{context.Request.Host}";
                    foreach (var c in context.Request.Cookies)
                    {
                        cookieContainer.Add(new Uri(uriString), new Cookie(c.Key, c.Value));
                    }
                    string jsonResult = string.Empty;
                    using (HttpClient client = new HttpClient(handler))
                    {
                        var res = await client.GetAsync($"{uriString}/.auth/me");
                        jsonResult = await res.Content.ReadAsStringAsync();
                    }
                    var obj = JArray.Parse(jsonResult);
                    string user_id = obj[0]["user_id"].Value<string>();
                    List<Claim> claims = new List<Claim>();
                    foreach (var claim in obj[0]["user_claims"])
                    {
                        claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
                    }
                    var identity = new GenericIdentity(azureAppServicePrincipalIdHeader);
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
