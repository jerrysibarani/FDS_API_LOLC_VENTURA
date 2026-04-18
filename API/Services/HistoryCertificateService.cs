using API.Data;
using API.IServices;
using API.Model;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class HistoryCertificateService(
        AppDbContext dbContext
    ) : IHistoryCertificateService
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<ResponseModel> GetHistoriesCertificateByBatch(int BatchID, Principal UserCurrent, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.HistoriesCertificates.Where(x => x.BATCH_NUMBER == BatchID).AsNoTracking().ToListAsync(cancellationToken);

            return new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
        }


        public async Task<ResponseModel> GetBatchHistoriesCertificate(Principal UserCurrent, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.HistoriesCertificates.GroupBy(x => x.BATCH_NUMBER).AsNoTracking().Select(x => new { x.Key }).OrderBy(x => x.Key).Select(s => s.Key).ToListAsync(cancellationToken);

            return new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
        }

    }
}
