using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WaterS.Application.Interfaces.Services;

namespace WaterS.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            UserName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            //UserRollId = httpContextAccessor.HttpContext?.User.Identity.
            UserRollName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        }

        public string UserId { get; }
        public string UserName { get; }
        public string UserRollId { get; }
        public string UserRollName { get; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}