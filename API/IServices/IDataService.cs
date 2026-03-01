using API.Model;
using API.Models.Params;
using API.Models;

namespace API.IServices
{
    public interface IDataService
    {
        Task<ResponseModel> GetForAHU(Principal currentUser, ParamData param);

        Task<ResponseModel> GetForCertificate(Principal currentUser, ParamData param);

        Task<ResponseModel> GetForMinuta(Principal currentUser, ParamData param);
    }
}
