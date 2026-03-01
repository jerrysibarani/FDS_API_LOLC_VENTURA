using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected virtual Principal currentUser
        {
            get
            {
                var identity = (ClaimsIdentity)HttpContext.User.Identity!;
                string superAdm = identity.FindFirst("IsSuperUser")?.Value?.ToString()!;
                bool isSuper = bool.TryParse(superAdm, out bool boolValue) ? boolValue : false;

                var principal = new Principal
                {
                    UID = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value!.ToString()!,
                    Username = identity.FindFirst(ClaimTypes.Name)?.Value!.ToString()!,
                    Email = identity.FindFirst(ClaimTypes.Email)?.Value!.ToLower()!,
                    UserType = identity.FindFirst("UserType")?.Value!.ToString()!,
                    ClientCode = identity.FindFirst("ClientCode")?.Value!.ToString()!,
                    CustomerCode = identity.FindFirst("CustomerCode")?.Value!.ToString()!,
                    IsSuperAdmin = isSuper
                };
               
                return principal;
            }
        }
    }
}
