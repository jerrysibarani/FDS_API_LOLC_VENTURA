using API.Data;
using API.Data.Models;
using API.Helpers;
using API.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class InvoiceService(
        AppDbContext DBContext
    ) : IInvoiceService
    {

        private readonly AppDbContext _dbContext = DBContext;

        public async Task<PriceDataDocument?> GetPriceDocumentCustomer(string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken = default)
        {
            var parameters = DbParameterHelper.CreateParameters(ClientCode, CustomerCode, StartDate, EndDate);

            return await _dbContext.Set<PriceDataDocument>()
                .FromSqlRaw("SELECT * FROM sp_get_invoice_customer(@ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            //var parameters = DbParameterHelper.CreateParameters(clientCode, customerCode, startDate, endDate);
            //return await _dbContext.Set<PriceDataDocument>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetInvoiceCustomer] @ClientCode, @CustomerCode, @StartDate, @EndDate", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
        }
        public async Task<int> ExeGenerateInvoiceCustomer(string UserId, string clientCode, string CustomerCode, DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken = default)
        {            
            var parameters = DbParameterHelper.CreateUserParameters(UserId, clientCode, CustomerCode, StartDate, EndDate);

            var result = await _dbContext.Set<ResponseTotal>()
                .FromSqlRaw("SELECT * FROM sp_create_invoice_customer(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return result?.Total ?? 0;

            //var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            //var result = await _dbContext.Set<ResponseTotal>()
            //    .FromSqlRaw("EXEC [dbo].[sp_CreateInvoiceCustomer] @UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate, UserId", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
            //return result?.Total ?? 0;
        }

    }
}
