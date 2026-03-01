using API.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IUserService
    {
        public Task<List<ApplicationUser>> GetAllUsersAsync();
        public Task<ApplicationUser> GetUserByIdAsync(string userId);
        public Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        public Task<IdentityResult> DeleteUserAsync(ApplicationUser user);


    }
}
