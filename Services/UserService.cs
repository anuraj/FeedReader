using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FeedReader.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UserService(IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }
        public string Name
        {
            get
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    return "Feed Reader";
                }

                return _httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }
        }

        public string Id
        {
            get
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    return "0000000";
                }

                return _httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    return true;
                }

                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            }
        }

    }
}