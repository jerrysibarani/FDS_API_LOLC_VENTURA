using API.Models;
using API.Data.Models;

namespace API.IServices
{
    public interface ICustomerService
    {
        public Task<IReadOnlyList<PostValueModels>> GetCodeCustomer(Principal UserCurrent, CancellationToken cancellationToken = default);
    }
}
