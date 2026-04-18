using API.Data.Entities;
using API.Data.Models;
using API.Models;

namespace API.IServices
{
    public interface IUserAccessService
    {
        public Task<List<ACCESSROLES>?> GetAllAccessUser(Principal UserCurrent, CancellationToken cancellationToken = default);
        public Task<ACCESSROLES?> GetAccessUserByPage(string PageName, Principal UserCurrent, CancellationToken cancellationToken = default);
        public Task<List<MenuAccessModels>> GetMenuAccesUser(Principal UserCurrent, CancellationToken cancellationToken = default);
        public Task<PersonalProfileModels> GetProfileUser(ApplicationUser UserCurrent, CancellationToken cancellationToken = default);        
    }
}
