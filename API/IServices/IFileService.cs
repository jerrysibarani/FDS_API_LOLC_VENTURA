using API.Models.Params;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.IServices
{
    public interface IFileService
    {
        public Task<bool> SaveCertificate(Principal UserCurrent, [FromBody] ParamFile Param, CancellationToken cancellationToken = default);
        public Task<bool> SaveMinuta(Principal UserCurrent, [FromBody] ParamFile Param, CancellationToken cancellationToken = default);
    }
}
