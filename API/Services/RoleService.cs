using API.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task<IdentityResult> CreateRoleAsync(string RoleName)
        {
            if (await RoleExistsAsync(RoleName))
                throw new InvalidOperationException("Role already exists.");

            var role = new IdentityRole(RoleName);
            return await _roleManager.CreateAsync(role);


        }


        public async Task<bool> DeleteRoleAsync(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                return true;
            } else
            {
                return false;
            }
        }
        public async Task<List<string?>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(role => role.Name).ToListAsync();
        }

        public async Task<List<IdentityRole>> GetIdentityRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
        public async Task<bool> RoleExistsAsync(string RoleName)
        {
            return await _roleManager.RoleExistsAsync(RoleName);
        }


    }
}
