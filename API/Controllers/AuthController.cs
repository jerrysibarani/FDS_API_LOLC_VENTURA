using API.Helpers;
using API.Model;
using API.IServices;
using API.Models.Params;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using API.Data.Entities;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        ILogger<AuthController> _logger,
        IConfiguration confRepo,
        IHttpContextAccessor _httpContextAccessor,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        IUserAccessService _userAccessService,
        IRoleUserService _roleUserService
        ) : ControllerBase
    {



        private readonly ILogger<AuthController> _logger = _logger;

        private readonly IConfiguration confRepo = confRepo;
        private readonly IHttpContextAccessor _httpContextAccessor = _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = _signInManager;
        private readonly IUserAccessService _userAccessService = _userAccessService;
        private readonly IRoleUserService _roleUserService = _roleUserService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ParamLogin prm)
        {
            try
            {
                _logger.LogInformation("Set Login {Time}", DateTime.UtcNow);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var username_ = prm.Username.IsNullOrEmpty() ? "" : prm.Username!.Trim().ToLower();
                ApplicationUser? user;
                if (prm.Username!.Contains("@"))
                    user = await _userManager.FindByEmailAsync(prm.Username);
                else
                    user = await _userManager.FindByNameAsync(prm.Username);

                _logger.LogInformation("Find Login by User or Email");
                if (user == null)
                {

                    _logger.LogInformation("Error : User Not Found");
                    return NotFound("User not found.");
                }
                if (user.EmailConfirmed)
                { 
                    if(user.USER_TYPE == ConstantaData.EXTERNAL)
                    {
                        _logger.LogInformation("Error : External");
                        return BadRequest("You not have access.");
                    }
                    var result = await _signInManager.CheckPasswordSignInAsync(user, prm.Password!, false);
                    if (!result.Succeeded)
                    {
                        _logger.LogInformation("Error : Sign Error Password");
                        return BadRequest("Password Wrong.");
                    }
                    var session = await _signInManager.PasswordSignInAsync(user, prm.Password!, isPersistent: false, lockoutOnFailure: false);
                    if (result.IsLockedOut)
                    {
                        _logger.LogInformation("Error : Locked");
                        return BadRequest("Login is Locked.");
                    }
                    else if (!session.Succeeded)
                    {
                        _logger.LogInformation("Error : Session");
                        return BadRequest("Invalid login attempt.");
                    }
                    else
                    {

                        var data = await _userAccessService.GetProfileUser();
                        var roles = await _roleUserService.GetAllRolesUserAsync(user.Id);
                        var role = roles.Select(x => x.RoleId).ToList();
                        List<Claim>? claims = CreateClaimUser(user, role!, data.IsSuperAdmin);
                        var token = CreateToken(claims!);
                        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                        string refreshToken = GenerateRefreshToken();
                        return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, jwtToken, 1, data), Formatting.Indented));
                    }
                }
                else
                {
                    _logger.LogInformation("Error : Email Not Confirm");
                    return BadRequest("User not actived.");      
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error :");
                return BadRequest(ex.Message);
                throw;
            }

        }

        

        [Authorize]
        [HttpPost("logout")]
        public async Task<object?> Logout([FromBody] ParamTokenRefresh request)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    await _signInManager.SignOutAsync();
                    await _httpContextAccessor.HttpContext!.SignOutAsync(IdentityConstants.ExternalScheme);
                    var principal = GetClaimsPrincipalExpiredToken(request.Token);
                    if (principal == null) return BadRequest("Invalid access token or refresh token!");
                    string userID = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                    var newToken = ResetToken(principal.Claims.ToList());
                    string jwtToken = new JwtSecurityTokenHandler().WriteToken(newToken);
                    string refToken = GenerateRefreshToken();
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, string.Empty), Formatting.Indented));
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

        [Authorize]
        [HttpPost("password")]
        public async Task<object?> Password(ParamPasswordCMS model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var UserLogin = _httpContextAccessor.HttpContext!.User;
                    var user = await _userManager.GetUserAsync(UserLogin);
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



        private static List<Claim>? CreateClaimUser(ApplicationUser userApp, List<string> roles, bool? isSuper)
        {
            List<Claim> claims = new()
            {
                 new Claim(ClaimTypes.NameIdentifier, userApp.Id ?? string.Empty),
                 new Claim(ClaimTypes.Name, userApp.UserName ?? string.Empty),
                 new Claim(ClaimTypes.Email, userApp.Email ?? string.Empty),
                 new Claim("UserType", userApp.USER_TYPE?.ToString() ?? string.Empty),
                 new Claim("ClientCode", userApp.CLIENT_CODE?.ToString() ?? string.Empty),
                 new Claim("CustomerCode", userApp.CUSTOMER_CODE?.ToString() ?? string.Empty),
                 new Claim("IsSuperUser", isSuper.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            _ = int.TryParse(confRepo.GetSection("JWT:HoursTokenValid").Value!.ToString(), out int _hoursValid);
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(confRepo.GetSection("JWT:Secret").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddHours(_hoursValid),
            signingCredentials: cred);
            return token;
        }

        private JwtSecurityToken ResetToken(List<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(confRepo.GetSection("JWT:Secret").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken
                (
                claims: authClaims,
                expires: DateTime.Now.AddHours(-24),
                signingCredentials: cred
                );
            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipalExpiredToken(string? token)
        {
            var tokenValid = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(confRepo.GetSection("JWT:Secret").Value!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValid, out SecurityToken? securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;

        }
    
    }
}
