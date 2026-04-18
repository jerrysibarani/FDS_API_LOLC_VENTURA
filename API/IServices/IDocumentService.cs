using API.Data.Models;
using API.Models.Views;
using API.Models;
using System.Data;
using API.Models.Params;
using API.Model;

namespace API.IServices
{
    public interface IDocumentService
    {

        public Task<bool> PutDocument(Principal UserCurrent, int id, DataModels Models, CancellationToken cancellationToken = default);
        
        public Task<bool> UpdateDocument(Principal UserCurrent, DataModels Models, CancellationToken cancellationToken = default);
        
        public Task<bool> DeleteDocument(Principal UserCurrent, int Id, CancellationToken cancellationToken = default);
        
        public Task<bool> StatusAhuDocument(Principal UserCurrent, ParamStatus Param, CancellationToken cancellationToken = default);
        
        public Task<bool> UpdateNotarisDocument(Principal UserCurrent, ParamNotaris Param, CancellationToken cancellationToken = default);
        
        


        public Task<ResponseModel> GenerateCertificates(Principal UserCurrent, List<CertificateModel> Param, CancellationToken cancellationToken = default);


        public Task<ResponseModel> UpdateCertificates(Principal UserCurrent, List<CertificateModel> Param, CancellationToken cancellationToken = default);





        public Task<IReadOnlyList<ColumnNameModel>> GetDocumentsColumnName(CancellationToken cancellationToken = default);
        public Task<DataTable> GetDocumentsCustomer(string UserId, string ClientCode, string CustomerCode, int StartTake, int EndTake, string? SearchColumn = null, string? SearchValue = null, CancellationToken cancellationToken = default);
        public Task<int> GetDocumentsCustomerTotal(string UserId, string ClientCode, string CustomerCode, string? SearchColumn = null, string? SearchValue = null, CancellationToken cancellationToken = default);

        public Task<DataTable> GetDocumentsClient(string UserId, string ClientCode, string CustomerCode, int StartTake, int EndTake, string? SearchColumn = null, string? SearchValue = null, CancellationToken cancellationToken = default);
        public Task<int> GetDocumentsClientTotal(string UserId, string ClientCode, string CustomerCode, string? SearchColumn = null, string? SearchValue = null, CancellationToken cancellationToken = default);
        
        public Task<DataTable> ExportDocumentsClient(string UserId, string ClientCode, string CustomerCode, DateTime StarDate, DateTime EndDate, CancellationToken cancellationToken = default);
        public Task<DataTable> ExportDocumentsCustomer(string UserId, string ClientCode, string CustomerCode, DateTime StarDate, DateTime EndDate, CancellationToken cancellationToken = default);
        
        public Task<PriceDataDocument?> GetInvoiceCustomer(string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken = default);
    }
}
