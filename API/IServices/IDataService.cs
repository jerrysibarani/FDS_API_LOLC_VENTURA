using API.Model;
using API.Models.Params;
using API.Models;

namespace API.IServices
{
    public interface IDataService
    {
        public Task<ResponseModel> GetForAHU_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);
        public Task<ResponseModel> GetForCertificate_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);
        public Task<ResponseModel> GetForMinuta_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);

        public Task<ResponseModel> GetForHistoryCertificates_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);




        public Task<ResponseModel> GetForAHU(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);
        public Task<ResponseModel> GetForCertificate(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);
        public Task<ResponseModel> GetForMinuta(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default);

    }
}
