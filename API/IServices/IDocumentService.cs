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

        Task<bool> PutDocument(Principal currentUser, int id, DataModels models);

        Task<bool> UpdateDocument(Principal currentUser, DataModels models);

        Task<bool> DeleteDocument(Principal currentUser, int id);

        Task<bool> StatusAhuDocument(Principal currentUser, ParamStatus param);

        Task<bool> UpdateNotarisDocument(Principal currentUser, ParamNotaris param);

        Task<bool> UpdateCertificateDocument(Principal currentUser, List<CertificateModel> param);

        Task<ResponseModel> UpdateCertificateDocumentModel(Principal currentUser, List<CertificateModel> param);

        Task<ResponseModel> UpdateVoucherDocumentModel(Principal currentUser, List<CertificateModel> param);

        Task<ResponseModel> GetDocumentModel(Principal currentUser, List<CertificateModel> param);



        Task<List<ColumnNameModel>> GetDocumentsColumnName();
        Task<DataTable> GetDocumentsCustomer(string UserId, string ClientCode, string CustomerCode, int StartTake, int EndTake, string? SearchColumn = null, string? SearchValue = null);
        Task<int> GetDocumentsCustomerTotal(string UserId, string ClientCode, string CustomerCode, string? SearchColumn = null, string? SearchValue = null);

        Task<DataTable> GetDocumentsClient(string UserId, string ClientCode, string CustomerCode, int StartTake, int EndTake, string? SearchColumn = null, string? SearchValue = null);
        Task<int> GetDocumentsClientTotal(string UserId, string ClientCode, string CustomerCode, string? SearchColumn = null, string? SearchValue = null);


        Task<DataTable> ExportDocumentsClient(string UserId, string ClientCode, string CustomerCode, DateTime StarDate, DateTime EndDate);
        Task<DataTable> ExportDocumentsCustomer(string UserId, string ClientCode, string CustomerCode, DateTime StarDate, DateTime EndDate);


        Task<PriceDataDocument?> GetInvoiceCustomer(string ClientCode, string CustomerCode, DateTime StartDate, DateTime EndDate);
    }
}
