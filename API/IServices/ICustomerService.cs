using API.Models.Views;
using API.Models;
using API.Data.Models;

namespace API.IServices
{
    public interface ICustomerService
    {
        Task<List<PostValueModels>> GetCodeCustomer(Principal currentUser);
    }
}
