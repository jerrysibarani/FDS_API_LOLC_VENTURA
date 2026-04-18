using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace API.Services
{
    public class RoleUserService(
        UserManager<ApplicationUser> _userManager,
        AppDbContext dbContext
    ) : IRoleUserService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<RolesUserModel>> GetAllRolesUserAsync(string UserId, CancellationToken cancellationToken)
        {
            return await _dbContext.UserRoles
                .Where(x => x.UserId == UserId)
                .Select(x => new RolesUserModel
                {
                    UserId = x.UserId,
                    RoleId = x.RoleId
                })
            .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(string UserId, string RoleName)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = $"User with ID {UserId} not found." });

            return await _userManager.AddToRoleAsync(user, RoleName.ToUpper());
        }

        public async Task<bool> DeleteUserRolesAsync(string UserId, string RoleName)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, RoleName);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRoleAsync(string UserId, string RoleName)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            return user != null && await _userManager.IsInRoleAsync(user, RoleName);
        }
    }

}
