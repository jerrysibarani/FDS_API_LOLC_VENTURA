using API.Data.Entities;
using API.Data.Models;

namespace API.IServices
{
    public interface IUserAccessService
    {
        public Task<bool> IsRoleSuperAdmin();
        public Task<ACCESSROLES?> GetAccessUserByPage(string pageName);
        public Task<List<MenuAccessModels>> GetMenuAccesUser();
        public Task<PersonalProfileModels> GetProfileUser();
        public Task<List<ACCESSROLES>?> GetAllAccessUser();

    }
}
