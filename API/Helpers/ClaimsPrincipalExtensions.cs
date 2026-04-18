using API.Models;
using System.Security.Claims;

namespace API.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Principal ToPrincipal(this ClaimsPrincipal user)
        {
            bool.TryParse(user.FindFirstValue("IsSuperUser"),out var isSuper);
            return new Principal
            {
                UID = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                Username = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Email = user.FindFirstValue(ClaimTypes.Email)?.ToLower() ?? string.Empty,
                UserType = user.FindFirstValue("UserType") ?? string.Empty,
                ClientCode = user.FindFirstValue("ClientCode") ?? string.Empty,
                CustomerCode = user.FindFirstValue("CustomerCode") ?? string.Empty,
                IsSuperAdmin = isSuper,
                User = user
            };
        }

    }
}
