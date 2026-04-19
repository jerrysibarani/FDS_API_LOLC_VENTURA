using API.Models;
using API.Data.Models;
using API.Data.Entities;

namespace API.IServices
{
    public interface ICustomerService
    {
        public Task<IReadOnlyList<PostValueModels>> GetCodeCustomer(Principal UserCurrent, CancellationToken cancellationToken = default);

        public Task<IReadOnlyList<CUSTOMER>> GetDataCustomer(Principal UserCurrent, CancellationToken cancellationToken = default);
    }
}
