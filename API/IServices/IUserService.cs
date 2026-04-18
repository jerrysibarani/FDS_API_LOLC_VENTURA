using API.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.IServices
{
    public interface IUserService
    {
        public Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        public Task<ApplicationUser> GetUserByIdAsync(string UserId);
        public Task<IdentityResult> UpdateUserAsync(ApplicationUser User);
        public Task<IdentityResult> DeleteUserAsync(ApplicationUser User);


    }
}
