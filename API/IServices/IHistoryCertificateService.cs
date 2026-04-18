using API.Model;
using API.Models;

namespace API.IServices
{
    public interface IHistoryCertificateService
    {

        public Task<ResponseModel> GetBatchHistoriesCertificate(Principal UserCurrent, CancellationToken cancellationToken = default);


        public Task<ResponseModel> GetHistoriesCertificateByBatch(int BatchID, Principal UserCurrent, CancellationToken cancellationToken = default);



    }
}
