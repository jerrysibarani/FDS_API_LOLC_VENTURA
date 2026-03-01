using API.Data;
using API.Data.Models;
using API.Helpers;
using API.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class InvoiceService(
        AppDbContext dbContext
    ) : IInvoiceService
    {

        private readonly AppDbContext _dbContext = dbContext;

        public async Task<PriceDataDocument?> GetPriceDocumentCustomer(string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {
            var parameters = DbParameterHelper.CreateParameters(clientCode, customerCode, startDate, endDate);

            return await _dbContext.Set<PriceDataDocument>()
                .FromSqlRaw("SELECT * FROM sp_get_invoice_customer(@ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            //var parameters = DbParameterHelper.CreateParameters(clientCode, customerCode, startDate, endDate);
            //return await _dbContext.Set<PriceDataDocument>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetInvoiceCustomer] @ClientCode, @CustomerCode, @StartDate, @EndDate", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
        }
        public async Task<int> ExeGenerateInvoiceCustomer(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {            
            var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);

            var result = await _dbContext.Set<ResponseTotal>()
                .FromSqlRaw("SELECT * FROM sp_create_invoice_customer(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();

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
