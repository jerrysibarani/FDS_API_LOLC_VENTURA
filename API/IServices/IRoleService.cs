using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IRoleService
    {
        public Task<IdentityResult> CreateRoleAsync(string roleName);
        public Task<List<string?>> GetAllRolesAsync();
        public Task<List<IdentityRole>> GetIdentityRolesAsync();
        public Task<bool> DeleteRoleAsync(string roleId);
        public Task<bool> RoleExistsAsync(string roleName);
    }
}
