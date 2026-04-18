using API.Data.Models;
using API.Models;

namespace API.IServices
{
    public interface INotarisService
    {
        public Task<IReadOnlyList<PostValueModels>> GetCodeNotaris(Principal UserCurrent, CancellationToken cancellationToken = default);

        public Task<bool> ChangeStatusNotaris(Principal UserCurrent, string Id, CancellationToken cancellationToken = default);
    }
}
