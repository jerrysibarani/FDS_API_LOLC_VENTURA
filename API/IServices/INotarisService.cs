using API.Data.Models;
using API.Models;

namespace API.IServices
{
    public interface INotarisService
    {
        Task<List<PostValueModels>> GetCodeNotaris(Principal currentUser);

        Task<bool> ChangeStatusNotaris(Principal currentUser, string id);
    }
}
