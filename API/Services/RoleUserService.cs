using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoleUserService(
        UserManager<ApplicationUser> _userManager,
        AppDbContext dbContext
    ) : IRoleUserService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<RolesUserModel>> GetAllRolesUserAsync(string userId)
        {
            return await _dbContext.UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => new RolesUserModel
                {
                    UserId = x.UserId,
                    RoleId = x.RoleId
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = $"User with ID {userId} not found." });

            return await _userManager.AddToRoleAsync(user, roleName.ToUpper());
        }

        public async Task<bool> DeleteUserRolesAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, roleName);
        }
    }

}
