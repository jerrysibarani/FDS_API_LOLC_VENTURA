using API.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IRoleUserService
    {
        public Task<List<RolesUserModel>> GetAllRolesUserAsync(string userId);
        public Task<IdentityResult> AssignRoleToUserAsync(string userId, string roleName);
        public Task<bool> DeleteUserRolesAsync(string userId, string roleName);
        public Task<bool> IsUserInRoleAsync(string userId, string roleName);


    }
}
