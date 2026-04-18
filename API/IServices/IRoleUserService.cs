using API.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IRoleUserService
    {
        public Task<List<RolesUserModel>> GetAllRolesUserAsync(string UserId, CancellationToken cancellationToken);
        public Task<IdentityResult> AssignRoleToUserAsync(string UserId, string RoleName);
        public Task<bool> DeleteUserRolesAsync(string UserId, string RoleName);
        public Task<bool> IsUserInRoleAsync(string UserId, string RoleName);


    }
}
