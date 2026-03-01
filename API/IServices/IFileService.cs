using API.Models.Params;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.IServices
{
    public interface IFileService
    {
        Task<bool> SaveCertificate(Principal currentUser, [FromBody] ParamFile param);

        Task<bool> SaveMinuta(Principal currentUser, [FromBody] ParamFile param);
    }
}
