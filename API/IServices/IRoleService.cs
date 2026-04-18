using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IRoleService
    {
        public Task<IdentityResult> CreateRoleAsync(string RoleName);
        public Task<List<string?>> GetAllRolesAsync();
        public Task<List<IdentityRole>> GetIdentityRolesAsync();
        public Task<bool> DeleteRoleAsync(string RoleId);
        public Task<bool> RoleExistsAsync(string RoleName);
    }
}
