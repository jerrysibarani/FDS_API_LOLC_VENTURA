using API.Data.Models;

namespace API.IServices
{
    public interface IInvoiceService
    {
        public Task<PriceDataDocument?> GetPriceDocumentCustomer(string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate);

        public Task<int> ExeGenerateInvoiceCustomer(string userId, string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate);


    }
}
