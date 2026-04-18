using API.Data.Entities;
using API.Model;
using API.Models.Params;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        IHttpContextAccessor _httpContextAccessor,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager
        ) : BaseController
    {

        private readonly IHttpContextAccessor _httpContextAccessor = _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = _signInManager;


        [HttpPost("password")]
        public async Task<object?> Password(ParamPasswordCMS model)
        {
            try
            {
                if (ModelState.IsValid && CurrentUser != null && CurrentUser.User != null)
                {
                    var user = await _userManager.GetUserAsync(CurrentUser.User);
                    if (user != null)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignOutAsync();
                            await _httpContextAccessor.HttpContext!.SignOutAsync(IdentityConstants.ExternalScheme);
                            return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, string.Empty), Formatting.Indented));
                        }
                        else
                        {
                            return BadRequest("Invalid Change Password.");
                        }
                    }
                    else
                    {
                        return BadRequest("User not Login.");
                    }
                }
                else
                {
                    return BadRequest("Parameter is null");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
