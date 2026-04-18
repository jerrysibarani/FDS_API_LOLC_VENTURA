using API.Data.Models;

namespace API.IServices
{
    public interface IInvoiceService
    {
        public Task<PriceDataDocument?> GetPriceDocumentCustomer(string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken = default);

        public Task<int> ExeGenerateInvoiceCustomer(string UserId, string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken = default);


    }
}
