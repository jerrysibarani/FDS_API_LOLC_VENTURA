using API.Models;
using API.Models.Params;
using API.Models.Views;

namespace API.IServices
{
    public interface IAHUService
    {
        public Task<IReadOnlyList<AhuAccountModels>> GetAhuAccounts(Principal UserCurrent, CancellationToken cancellationToken = default);

        public Task<bool> ChangePasswordAHU(Principal UserCurrent, ParamPasswordAhu Param, CancellationToken cancellationToken = default);
    }
}
