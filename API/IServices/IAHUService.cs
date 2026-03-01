using API.Models;
using API.Models.Params;
using API.Models.Views;

namespace API.IServices
{
    public interface IAHUService
    {
        Task<List<AhuAccountModels>> GetAhuAccounts(Principal currentUser);

        Task<bool> ChangePasswordAHU(Principal currentUser, ParamPasswordAhu param);
    }
}
