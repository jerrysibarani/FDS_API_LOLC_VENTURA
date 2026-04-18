using API.Data.Entities;
using API.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager
    ) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string UserId)
        {
            return await _userManager.FindByIdAsync(UserId) ?? new ApplicationUser();
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser Users)
        {
            return await _userManager.UpdateAsync(Users);
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser Users)
        {
            return await _userManager.DeleteAsync(Users);
        }


    }
}
