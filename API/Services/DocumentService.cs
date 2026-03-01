using API.Data.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using API.Helpers;
using API.IServices;
using API.Models;
using API.Helpers;
using API.Models.Views;
using AutoMapper;
using API.Models.Params;
using API.Data.Entities;
using API.Model;

namespace API.Services
{
    public class DocumentService(
        ICoreService coreService,
        AppDbContext dbContext,
        IMapper _mapper,
        IConfiguration configuration
    ) : IDocumentService
    {

        private readonly IMapper _mapper = _mapper;
        private readonly ICoreService _coreService = coreService;
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IConfiguration _configuration = configuration;

        private int batchSize { get; set; }

        private int GetBatchSizeFromConfig()
        {
            var value = _configuration["MySettings:BatchList"];
            return int.TryParse(value, out int result) && result > 0 ? result : 250;
        }


        public async Task<bool> PutDocument(Principal currentUser, int id, DataModels models)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(id) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }
            else
            {
                _mapper.Map(models, result);
                result.MODIFIED_BY = currentUser.Email;
                result.MODIFIED_DATE = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(models.BRANCH_CODE) && models.BRANCH_CODE.Length > 5)
                {
                    result.BRANCH_CODE = models.BRANCH_NAME;
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }


        public async Task<bool> UpdateDocument(Principal currentUser, DataModels models)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(models.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }
            else
            {
                _mapper.Map(models, result);
                result.MODIFIED_BY = currentUser.Email;
                result.MODIFIED_DATE = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(models.BRANCH_CODE) && models.BRANCH_CODE.Length > 5)
                {
                    result.BRANCH_CODE = models.BRANCH_NAME;
                }
                await _dbContext.SaveChangesAsync();
                return true;

            }
        }

        public async Task<bool> DeleteDocument(Principal currentUser, int id)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(id) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }
            else
            {
                result.DELETED_BY = currentUser.Email;
                result.DELETED_DATE = DateTime.UtcNow;
                result.DELETED_STATUS = true;
                _dbContext.Documents.Update(result);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> StatusAhuDocument(Principal currentUser, ParamStatus param)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(param.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();

            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }

            var result = await joinedResults.FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(param.NOTARIS_CODE) && string.IsNullOrEmpty(result.NOTARIS_CODE))
                {
                    result.NOTARIS_CODE = param.NOTARIS_CODE;
                }

                if (!string.IsNullOrEmpty(param.NO_VOUCHER))
                {
                    result.NO_VOUCHER = param.NO_VOUCHER;
                }

                if (!string.IsNullOrEmpty(param.MESSAGES) && param.MESSAGES.Contains("Terdaftar"))
                {
                    result.MESSAGES = param.MESSAGES;
                }
                else
                {
                    result.AHU_BY = currentUser.Email;
                    result.AHU_DATE = DateTime.UtcNow;
                    result.AHU_STATUS = true;
                    result.IS_DUPLICATE = false;
                }
                _dbContext.Documents.Update(result);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateNotarisDocument(Principal currentUser, ParamNotaris param)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(param.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();

            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }

            var result = await joinedResults.FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }
            else
            {
                if (!String.IsNullOrEmpty(param.NOTARIS_CODE))
                {
                    result.NOTARIS_CODE = param.NOTARIS_CODE;
                    result.MODIFIED_BY = currentUser.Email;
                    result.MODIFIED_DATE = DateTime.UtcNow;
                    _dbContext.Documents.Update(result);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
        }


        public async Task<bool> UpdateCertificateDocument(Principal currentUser, List<CertificateModel> param)
        {
            try
            {
                batchSize = GetBatchSizeFromConfig();
                var allUpdatedDocuments = new List<DOCUMENTS>();

                for (int i = 0; i < param.Count; i += batchSize)
                {
                    var batch = param.Skip(i).Take(batchSize).ToList();
                    var ids = batch.Select(x => x.ID).ToList();

                    var documents = await _dbContext.Documents
                        .Where(p => ids.Contains(p.ID) && !p.DELETED_STATUS && !string.IsNullOrEmpty(p.NOMOR_AKTA))
                        .ToListAsync();

                    if (documents.Any())
                    {
                        foreach (var doc in documents)
                        {
                            var match = batch.FirstOrDefault(x =>
                                x.ID == doc.ID &&
                                (
                                    x.NO_VOUCHER == doc.NO_VOUCHER ||
                                    (
                                        x.CLIENT_CODE == doc.CLIENT_CODE &&
                                        x.CUSTOMER_CODE == doc.CUSTOMER_CODE &&
                                        x.NOTARIS_CODE == doc.NOTARIS_CODE &&
                                        x.NOMOR_AKTA == doc.NOMOR_AKTA &&
                                        x.TANGGAL_AKTA?.Date == doc.TANGGAL_AKTA?.Date
                                    )
                                ));

                            if (match != null)
                            {
                                if (match.StatusCertificate)
                                {
                                    doc.NO_SERTIFIKAT = match.NO_SERTIFIKAT;
                                    doc.TGL_SERTIFIKAT = Convert.ToDateTime(match.WaktuDaftar);
                                    doc.CERTIFICATE_STATUS = true;
                                    doc.CERTIFICATE_BY = currentUser.Email;
                                    doc.CERTIFICATE_DATE = DateTime.UtcNow;
                                }

                                doc.NO_VOUCHER = match.NO_VOUCHER ?? doc.NO_VOUCHER;
                                doc.NO_REGISTRASI = match.NO_REGISTRASI ?? doc.NO_REGISTRASI;
                                doc.NO_PEMBIAYAAN = match.NO_PEMBIAYAAN ?? doc.NO_PEMBIAYAAN;
                                allUpdatedDocuments.Add(doc);
                            }
                        }
                    }
                }
                // Update dan simpan semua dokumen sekaligus
                if (allUpdatedDocuments.Any())
                {
                    _dbContext.Documents.UpdateRange(allUpdatedDocuments);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            } catch (Exception) 
            {
                return false;
            }
        }

        public async Task<ResponseModel> UpdateCertificateDocumentModel(Principal currentUser, List<CertificateModel> param)
        {
            try
            {
                batchSize = GetBatchSizeFromConfig();
                var allUpdatedDocuments = new List<DOCUMENTS>();
                var result = new List<CertificateModel>();
                for (int i = 0; i < param.Count; i += batchSize)
                {
                    var batch = param.Skip(i).Take(batchSize).ToList();

                    // 1. Prepare HashSets for filtering
                    var nomorAktaSet = batch.Select(p => p.NOMOR_AKTA!).ToHashSet();
                    var tanggalAktaSet = batch.Where(p => p.TANGGAL_AKTA.HasValue).Select(p => p.TANGGAL_AKTA!.Value.Date).ToHashSet();
                    var namaPemberiFidusiaSet = batch.Select(p => p.NAMA_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var npwpPemberiFidusiaSet = batch.Where(p => !string.IsNullOrEmpty(p.NPWP_PEMBERIFIDUSIA)).Select(p => p.NPWP_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    var notarisNameSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NOTARIS_NAME)).Select(p => p.NOTARIS_NAME!.ToString().Trim().ToUpper()).ToHashSet();
                    var namaPenerimaFidusiaSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NAMA_PENERIMAFIDUSIA)).Select(p => p.NAMA_PENERIMAFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var clientCodeSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.CLIENT_CODE)).Select(p => p.CLIENT_CODE!.ToString().Trim().ToUpper()).ToHashSet();
                    var nomorVoucher = batch.Select(p => p.NO_VOUCHER!.ToString().Trim().ToUpper()).ToHashSet();

                    // 2. Build base query
                    var baseQuery = (from d in _dbContext.Documents
                                     join n in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { n.NOTARIS_CODE, n.CLIENT_CODE } // d.NOTARIS_CODE equals n.NOTARIS_CODE
                                     join c in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { c.CUSTOMER_CODE, c.CLIENT_CODE }
                                     join l in _dbContext.Clients on d.CLIENT_CODE equals l.CLIENT_CODE
                                     join b in _dbContext.Pnbps on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { b.CUSTOMER_CODE, b.CLIENT_CODE }
                                     join f in _dbContext.ClientFees on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { f.CUSTOMER_CODE, f.CLIENT_CODE }
                                     join br in _dbContext.Branches on d.BRANCH_CODE equals br.BRANCH_CODE into bcDefault
                                     from br in bcDefault.DefaultIfEmpty()
                                     where !d.DELETED_STATUS
                                            && d.AHU_STATUS
                                            && c.ISACTIVE
                                            && l.ISACTIVE
                                            && d.TANGGAL_AKTA.HasValue
                                            //&& !d.CERTIFICATE_STATUS
                                            && b.ISACTIVE && (d.NILAI_JAMINAN >= b.MIN_VALUE && d.NILAI_JAMINAN <= b.MAX_VALUE)
                                            && f.ISACTIVE

                                     select new DataAhuModels
                                     {
                                         ID = d.ID,
                                         CLIENT_CODE = d.CLIENT_CODE,
                                         CLIENT_NAME = l.CLIENT_NAME,
                                         CUSTOMER_CODE = d.CUSTOMER_CODE,
                                         CUSTOMER_NAME = c.CUSTOMER_NAME,
                                         CUSTOMER_NPWP = c.NPWP,
                                         BRANCH_CODE = d.BRANCH_CODE,
                                         BRANCH_NAME = br.BRANCH_CODE == null ? d.BRANCH_CODE : br.BRANCH_NAME,
                                         NO_VOUCHER = d.NO_VOUCHER,
                                         NOTARIS_CODE = d.NOTARIS_CODE,
                                         NOTARIS_NAME = n.NOTARIS_NAME,
                                         NOMOR_AKTA = d.NOMOR_AKTA,
                                         TANGGAL_AKTA = d.TANGGAL_AKTA,
                                         NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                         NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                         NILAI_JAMINAN = d.NILAI_JAMINAN,
                                         PRICE_PNBP = b.PRICE,
                                         FEE_COST = f.FEE_COST,
                                         TAX_COST = f.TAX_COST
                                     }).AsQueryable();

                    // 3. Apply user type filter
                    if (currentUser.UserType == ConstantaData.INTERNAL)
                    {
                        baseQuery = baseQuery.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
                    }

                    baseQuery = baseQuery
                                .Where(d => (!string.IsNullOrEmpty(d.NO_VOUCHER) && nomorVoucher.Count > 0 && nomorVoucher.Contains(d.NO_VOUCHER.Trim().ToUpper()))
                                  || ((!string.IsNullOrEmpty(d.NOMOR_AKTA) && nomorAktaSet.Count > 0 && nomorAktaSet.Contains(d.NOMOR_AKTA.Trim().ToUpper()))
                                  && (d.TANGGAL_AKTA.HasValue && tanggalAktaSet.Contains(d.TANGGAL_AKTA.Value.Date))
                                  && (!string.IsNullOrEmpty(d.NOTARIS_NAME) && notarisNameSet.Count > 0 && notarisNameSet.Contains(d.NOTARIS_NAME.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.NAMA_PEMBERIFIDUSIA) && namaPemberiFidusiaSet.Count > 0 && namaPemberiFidusiaSet.Contains(d.NAMA_PEMBERIFIDUSIA.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.CUSTOMER_NAME) && namaPenerimaFidusiaSet.Count > 0 && namaPenerimaFidusiaSet.Contains(d.CUSTOMER_NAME.Trim().ToUpper()))
                                  //&& (!string.IsNullOrEmpty(d.CLIENT_CODE) && clientCodeSet.Count > 0 && clientCodeSet.Contains(d.CLIENT_CODE.Trim().ToUpper()))
                                  )
                                );

                    // 4. Execute query and filter in memory
                    var allDataDocList = await baseQuery.AsNoTracking().ToListAsync();
                    
                    //Join all
                    var joinedResults = param
                                        .SelectMany(p => allDataDocList
                                            .Where(r =>
                                                (p.NO_VOUCHER?.Trim() == r.NO_VOUCHER?.Trim()) ||
                                                (
                                                    p.NOMOR_AKTA?.Trim() == r.NOMOR_AKTA?.Trim() &&
                                                    (p.TANGGAL_AKTA.HasValue && r.TANGGAL_AKTA.HasValue && p.TANGGAL_AKTA.Value.Date == r.TANGGAL_AKTA.Value.Date) &&
                                                    p.NOTARIS_NAME?.Trim().ToUpper() == r.NOTARIS_NAME?.Trim().ToUpper() &&
                                                    p.NAMA_PEMBERIFIDUSIA?.Trim().ToUpper() == r.NAMA_PEMBERIFIDUSIA?.Trim() &&
                                                    p.NAMA_PENERIMAFIDUSIA?.Trim().ToUpper() == r.CUSTOMER_NAME?.Trim().ToUpper()
                                                )
                                            )
                                            .DefaultIfEmpty(),
                                            (p, r) => new CertificateModel
                                            {
                                                ID = r?.ID ?? 0,
                                                CLIENT_CODE = r?.CLIENT_CODE ?? p.CLIENT_CODE,
                                                CUSTOMER_CODE = r?.CUSTOMER_CODE ?? p.CUSTOMER_CODE,
                                                BRANCH_CODE = r?.BRANCH_CODE ?? p.BRANCH_CODE,
                                                BRANCH_NAME = r?.BRANCH_NAME ?? p.BRANCH_NAME,
                                                NOMOR_AKTA = r?.NOMOR_AKTA ?? p.NOMOR_AKTA,
                                                TANGGAL_AKTA = r?.TANGGAL_AKTA ?? p.TANGGAL_AKTA,
                                                NOTARIS_CODE = r?.NOTARIS_CODE ?? p.NOTARIS_CODE,
                                                NOTARIS_NAME = r?.NOTARIS_NAME ?? p.NOTARIS_NAME,
                                                NAMA_PEMBERIFIDUSIA = r?.NAMA_PEMBERIFIDUSIA ?? p.NAMA_PEMBERIFIDUSIA,
                                                NPWP_PEMBERIFIDUSIA = r?.NPWP_PEMBERIFIDUSIA ?? p.NPWP_PEMBERIFIDUSIA,
                                                NAMA_PENERIMAFIDUSIA = r?.CUSTOMER_NAME ?? p.NAMA_PENERIMAFIDUSIA,
                                                NPWP_PENERIMAFIDUSIA = r?.CUSTOMER_NPWP ?? p.NPWP_PENERIMAFIDUSIA,
                                                CODE_PENERIMAFIDUSIA = r?.CUSTOMER_CODE ?? p.CODE_PENERIMAFIDUSIA,
                                                NILAI_JAMINAN = r?.NILAI_JAMINAN ?? p.NILAI_JAMINAN,
                                                PRICE_PNBP = r?.PRICE_PNBP ?? p.PRICE_PNBP,
                                                FEE_COST = r?.FEE_COST ?? p.FEE_COST,
                                                TAX_COST = r?.TAX_COST ?? p.TAX_COST,
                                                NO_VOUCHER = p.NO_VOUCHER ?? r?.NO_VOUCHER,
                                                NO_SERTIFIKAT = p.NO_SERTIFIKAT,
                                                NO_REGISTRASI = p.NO_REGISTRASI,
                                                NO_PEMBIAYAAN = p.NO_PEMBIAYAAN,
                                                StatusCertificate = p.StatusCertificate,
                                                NameFileCertificate = p.NameFileCertificate,
                                                TypeFidusia = p.TypeFidusia,
                                                WILAYAH = p.WILAYAH,
                                                WaktuDaftar = p.WaktuDaftar,
                                            })
                                        .ToList();

                    var listDoc = joinedResults.Where(x => x.ID > 0 && !string.IsNullOrEmpty(x.NO_VOUCHER)).ToList();
                    if (listDoc.Count > 0)
                    {
                        var idsToUpdate = listDoc.Select(x => x.ID).ToList();
                        var documentsToUpdate = await _dbContext.Documents.Where(x => idsToUpdate.Contains(x.ID) && !x.DELETED_STATUS && !string.IsNullOrEmpty(x.NOMOR_AKTA)).ToListAsync();
                        if (documentsToUpdate.Any())
                        {
                            foreach (var doc in documentsToUpdate)
                            {
                                var match = listDoc.FirstOrDefault(x =>
                                            x.ID == doc.ID &&
                                            (x.NO_VOUCHER == doc.NO_VOUCHER ||
                                            (x.CLIENT_CODE == doc.CLIENT_CODE &&
                                              x.CUSTOMER_CODE == doc.CUSTOMER_CODE &&
                                              x.NOTARIS_CODE == doc.NOTARIS_CODE &&
                                              x.NOMOR_AKTA == doc.NOMOR_AKTA &&
                                              x.TANGGAL_AKTA?.Date == doc.TANGGAL_AKTA?.Date
                                            )));

                                if (match != null)
                                {
                                    if (match.StatusCertificate)
                                    {
                                        doc.NO_SERTIFIKAT = match.NO_SERTIFIKAT;
                                        doc.TGL_SERTIFIKAT = Convert.ToDateTime(match.WaktuDaftar);
                                        doc.CERTIFICATE_STATUS = true;
                                        doc.CERTIFICATE_BY = currentUser.Email;
                                        doc.CERTIFICATE_DATE = DateTime.UtcNow;
                                    }
                                    doc.NO_VOUCHER = match.NO_VOUCHER ?? doc.NO_VOUCHER;
                                    doc.NO_REGISTRASI = match.NO_REGISTRASI ?? doc.NO_REGISTRASI;
                                    doc.NO_PEMBIAYAAN = match.NO_PEMBIAYAAN ?? doc.NO_PEMBIAYAAN;

                                    allUpdatedDocuments.Add(doc);
                                }
                            }
                        }
                    }
                    
                    result.AddRange(joinedResults);
                }

                // Update dan simpan semua dokumen sekaligus
                if (allUpdatedDocuments.Any())
                {
                    _dbContext.Documents.UpdateRange(allUpdatedDocuments);
                    await _dbContext.SaveChangesAsync();
                }
                var response = new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel(ResponseCode.Error, "Error", 0, ex.Message);
            }
        }

        public async Task<ResponseModel> UpdateVoucherDocumentModel(Principal currentUser, List<CertificateModel> param)
        {
            try
            {
                batchSize = GetBatchSizeFromConfig();
                var allUpdatedDocuments = new List<DOCUMENTS>();
                var result = new List<CertificateModel>();
                for (int i = 0; i < param.Count; i += batchSize)
                {
                    var batch = param.Skip(i).Take(batchSize).ToList();

                    // 1. Prepare HashSets for filtering
                    var nomorAktaSet = batch.Select(p => p.NOMOR_AKTA!).ToHashSet();
                    var tanggalAktaSet = batch.Where(p => p.TANGGAL_AKTA.HasValue).Select(p => p.TANGGAL_AKTA!.Value.Date).ToHashSet();
                    var namaPemberiFidusiaSet = batch.Select(p => p.NAMA_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var npwpPemberiFidusiaSet = batch.Where(p => !string.IsNullOrEmpty(p.NPWP_PEMBERIFIDUSIA)).Select(p => p.NPWP_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    var notarisNameSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NOTARIS_NAME)).Select(p => p.NOTARIS_NAME!.ToString().Trim().ToUpper()).ToHashSet();
                    var namaPenerimaFidusiaSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NAMA_PENERIMAFIDUSIA)).Select(p => p.NAMA_PENERIMAFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var clientCodeSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.CLIENT_CODE)).Select(p => p.CLIENT_CODE!.ToString().Trim().ToUpper()).ToHashSet();
                    var nomorVoucher = batch.Select(p => p.NO_VOUCHER!.ToString().Trim().ToUpper()).ToHashSet();

                    // 2. Build base query
                    var baseQuery = (from d in _dbContext.Documents
                                     join n in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { n.NOTARIS_CODE, n.CLIENT_CODE } // d.NOTARIS_CODE equals n.NOTARIS_CODE
                                     join c in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { c.CUSTOMER_CODE, c.CLIENT_CODE }
                                     join l in _dbContext.Clients on d.CLIENT_CODE equals l.CLIENT_CODE
                                     join b in _dbContext.Pnbps on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { b.CUSTOMER_CODE, b.CLIENT_CODE }
                                     join f in _dbContext.ClientFees on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { f.CUSTOMER_CODE, f.CLIENT_CODE }
                                     join br in _dbContext.Branches on d.BRANCH_CODE equals br.BRANCH_CODE into bcDefault
                                     from br in bcDefault.DefaultIfEmpty()
                                     where !d.DELETED_STATUS
                                            && d.AHU_STATUS
                                            && c.ISACTIVE
                                            && l.ISACTIVE
                                            && d.TANGGAL_AKTA.HasValue
                                            //&& !d.CERTIFICATE_STATUS
                                            && b.ISACTIVE && (d.NILAI_JAMINAN >= b.MIN_VALUE && d.NILAI_JAMINAN <= b.MAX_VALUE)
                                            && f.ISACTIVE

                                     select new DataAhuModels
                                     {
                                         ID = d.ID,
                                         CLIENT_CODE = d.CLIENT_CODE,
                                         CLIENT_NAME = l.CLIENT_NAME,
                                         CUSTOMER_CODE = d.CUSTOMER_CODE,
                                         CUSTOMER_NAME = c.CUSTOMER_NAME,
                                         CUSTOMER_NPWP = c.NPWP,
                                         BRANCH_CODE = d.BRANCH_CODE,
                                         BRANCH_NAME = br.BRANCH_CODE == null ? d.BRANCH_CODE : br.BRANCH_NAME,
                                         NO_VOUCHER = d.NO_VOUCHER,
                                         NOTARIS_CODE = d.NOTARIS_CODE,
                                         NOTARIS_NAME = n.NOTARIS_NAME,
                                         NOMOR_AKTA = d.NOMOR_AKTA,
                                         TANGGAL_AKTA = d.TANGGAL_AKTA,
                                         NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                         NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                         NILAI_JAMINAN = d.NILAI_JAMINAN,
                                         PRICE_PNBP = b.PRICE,
                                         FEE_COST = f.FEE_COST,
                                         TAX_COST = f.TAX_COST
                                     }).AsQueryable();

                    // 3. Apply user type filter
                    if (currentUser.UserType == ConstantaData.INTERNAL)
                    {
                        baseQuery = baseQuery.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
                    }

                    baseQuery = baseQuery
                                .Where(d => (!string.IsNullOrEmpty(d.NO_VOUCHER) && nomorVoucher.Count > 0 && nomorVoucher.Contains(d.NO_VOUCHER.Trim().ToUpper()))
                                  || ((!string.IsNullOrEmpty(d.NOMOR_AKTA) && nomorAktaSet.Count > 0 && nomorAktaSet.Contains(d.NOMOR_AKTA.Trim().ToUpper()))
                                  && (d.TANGGAL_AKTA.HasValue && tanggalAktaSet.Contains(d.TANGGAL_AKTA.Value.Date))
                                  && (!string.IsNullOrEmpty(d.NOTARIS_NAME) && notarisNameSet.Count > 0 && notarisNameSet.Contains(d.NOTARIS_NAME.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.NAMA_PEMBERIFIDUSIA) && namaPemberiFidusiaSet.Count > 0 && namaPemberiFidusiaSet.Contains(d.NAMA_PEMBERIFIDUSIA.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.CUSTOMER_NAME) && namaPenerimaFidusiaSet.Count > 0 && namaPenerimaFidusiaSet.Contains(d.CUSTOMER_NAME.Trim().ToUpper()))
                                  //&& (!string.IsNullOrEmpty(d.CLIENT_CODE) && clientCodeSet.Count > 0 && clientCodeSet.Contains(d.CLIENT_CODE.Trim().ToUpper()))
                                  )
                                );

                    // 4. Execute query and filter in memory
                    var allDataDocList = await baseQuery.AsNoTracking().ToListAsync();

                    //Join all
                    var joinedResults = param
                                        .SelectMany(p => allDataDocList
                                            .Where(r =>
                                                (p.NO_VOUCHER?.Trim() == r.NO_VOUCHER?.Trim()) ||
                                                (
                                                    p.NOMOR_AKTA?.Trim() == r.NOMOR_AKTA?.Trim() &&
                                                    (p.TANGGAL_AKTA.HasValue && r.TANGGAL_AKTA.HasValue && p.TANGGAL_AKTA.Value.Date == r.TANGGAL_AKTA.Value.Date) &&
                                                    p.NOTARIS_NAME?.Trim().ToUpper() == r.NOTARIS_NAME?.Trim().ToUpper() &&
                                                    p.NAMA_PEMBERIFIDUSIA?.Trim().ToUpper() == r.NAMA_PEMBERIFIDUSIA?.Trim() &&
                                                    p.NAMA_PENERIMAFIDUSIA?.Trim().ToUpper() == r.CUSTOMER_NAME?.Trim().ToUpper()
                                                )
                                            )
                                            .DefaultIfEmpty(),
                                            (p, r) => new CertificateModel
                                            {
                                                ID = r?.ID ?? 0,
                                                CLIENT_CODE = r?.CLIENT_CODE ?? p.CLIENT_CODE,
                                                CUSTOMER_CODE = r?.CUSTOMER_CODE ?? p.CUSTOMER_CODE,
                                                BRANCH_CODE = r?.BRANCH_CODE ?? p.BRANCH_CODE,
                                                BRANCH_NAME = r?.BRANCH_NAME ?? p.BRANCH_NAME,
                                                NOMOR_AKTA = r?.NOMOR_AKTA ?? p.NOMOR_AKTA,
                                                TANGGAL_AKTA = r?.TANGGAL_AKTA ?? p.TANGGAL_AKTA,
                                                NOTARIS_CODE = r?.NOTARIS_CODE ?? p.NOTARIS_CODE,
                                                NOTARIS_NAME = r?.NOTARIS_NAME ?? p.NOTARIS_NAME,
                                                NAMA_PEMBERIFIDUSIA = r?.NAMA_PEMBERIFIDUSIA ?? p.NAMA_PEMBERIFIDUSIA,
                                                NPWP_PEMBERIFIDUSIA = r?.NPWP_PEMBERIFIDUSIA ?? p.NPWP_PEMBERIFIDUSIA,
                                                NAMA_PENERIMAFIDUSIA = r?.CUSTOMER_NAME ?? p.NAMA_PENERIMAFIDUSIA,
                                                NPWP_PENERIMAFIDUSIA = r?.CUSTOMER_NPWP ?? p.NPWP_PENERIMAFIDUSIA,
                                                CODE_PENERIMAFIDUSIA = r?.CUSTOMER_CODE ?? p.CODE_PENERIMAFIDUSIA,
                                                NILAI_JAMINAN = r?.NILAI_JAMINAN ?? p.NILAI_JAMINAN,
                                                PRICE_PNBP = r?.PRICE_PNBP ?? p.PRICE_PNBP,
                                                FEE_COST = r?.FEE_COST ?? p.FEE_COST,
                                                TAX_COST = r?.TAX_COST ?? p.TAX_COST,
                                                NO_VOUCHER = p.NO_VOUCHER ?? r?.NO_VOUCHER,
                                                NO_SERTIFIKAT = p.NO_SERTIFIKAT,
                                                NO_REGISTRASI = p.NO_REGISTRASI,
                                                NO_PEMBIAYAAN = p.NO_PEMBIAYAAN,
                                                StatusCertificate = p.StatusCertificate,
                                                NameFileCertificate = p.NameFileCertificate,
                                                TypeFidusia = p.TypeFidusia,
                                                WILAYAH = p.WILAYAH,
                                                WaktuDaftar = p.WaktuDaftar,
                                            })
                                        .ToList();

                    var listDoc = joinedResults.Where(x => x.ID > 0 && !string.IsNullOrEmpty(x.NO_VOUCHER)).ToList();
                    if (listDoc.Count > 0)
                    {
                        var idsToUpdate = listDoc.Select(x => x.ID).ToList();
                        var documentsToUpdate = await _dbContext.Documents.Where(x => idsToUpdate.Contains(x.ID) && !x.DELETED_STATUS && !string.IsNullOrEmpty(x.NOMOR_AKTA)).ToListAsync();
                        if (documentsToUpdate.Any())
                        {
                            foreach (var doc in documentsToUpdate)
                            {
                                var match = listDoc.FirstOrDefault(x =>
                                            x.ID == doc.ID &&
                                            (x.NO_VOUCHER == doc.NO_VOUCHER ||
                                            (x.CLIENT_CODE == doc.CLIENT_CODE &&
                                              x.CUSTOMER_CODE == doc.CUSTOMER_CODE &&
                                              x.NOTARIS_CODE == doc.NOTARIS_CODE &&
                                              x.NOMOR_AKTA == doc.NOMOR_AKTA &&
                                              x.TANGGAL_AKTA?.Date == doc.TANGGAL_AKTA?.Date
                                            )));

                                if (match != null)
                                {
                                    doc.NO_VOUCHER = match.NO_VOUCHER ?? doc.NO_VOUCHER;
                                    allUpdatedDocuments.Add(doc);
                                }
                            }
                        }
                    }

                    result.AddRange(joinedResults);
                }

                // Update dan simpan semua dokumen sekaligus
                if (allUpdatedDocuments.Any())
                {
                    _dbContext.Documents.UpdateRange(allUpdatedDocuments);
                    await _dbContext.SaveChangesAsync();
                }
                var response = new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel(ResponseCode.Error, "Error", 0, ex.Message);
            }
        }


        public async Task<ResponseModel> GetDocumentModel(Principal currentUser, List<CertificateModel> param)
        {
            try
            {
                batchSize = GetBatchSizeFromConfig();
                var result = new List<CertificateModel>();
                for (int i = 0; i < param.Count; i += batchSize)
                {
                    var batch = param.Skip(i).Take(batchSize).ToList();

                    // 1. Prepare HashSets for filtering
                    var nomorAktaSet = batch.Select(p => p.NOMOR_AKTA!).ToHashSet();
                    var tanggalAktaSet = batch.Where(p => p.TANGGAL_AKTA.HasValue).Select(p => p.TANGGAL_AKTA!.Value.Date).ToHashSet();
                    var namaPemberiFidusiaSet = batch.Select(p => p.NAMA_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var npwpPemberiFidusiaSet = batch.Where(p => !string.IsNullOrEmpty(p.NPWP_PEMBERIFIDUSIA)).Select(p => p.NPWP_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    var notarisNameSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NOTARIS_NAME)).Select(p => p.NOTARIS_NAME!.ToString().Trim().ToUpper()).ToHashSet();
                    var namaPenerimaFidusiaSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.NAMA_PENERIMAFIDUSIA)).Select(p => p.NAMA_PENERIMAFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var clientCodeSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.CLIENT_CODE)).Select(p => p.CLIENT_CODE!.ToString().Trim().ToUpper()).ToHashSet();
                    var nomorVoucher = batch.Select(p => p.NO_VOUCHER!.ToString().Trim().ToUpper()).ToHashSet();

                    // 2. Build base query
                    var baseQuery = (from d in _dbContext.Documents
                                     join n in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { n.NOTARIS_CODE, n.CLIENT_CODE } // d.NOTARIS_CODE equals n.NOTARIS_CODE
                                     join c in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { c.CUSTOMER_CODE, c.CLIENT_CODE }
                                     join l in _dbContext.Clients on d.CLIENT_CODE equals l.CLIENT_CODE
                                     join b in _dbContext.Pnbps on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { b.CUSTOMER_CODE, b.CLIENT_CODE }
                                     join f in _dbContext.ClientFees on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { f.CUSTOMER_CODE, f.CLIENT_CODE }
                                     join br in _dbContext.Branches on d.BRANCH_CODE equals br.BRANCH_CODE into bcDefault
                                     from br in bcDefault.DefaultIfEmpty()
                                     where !d.DELETED_STATUS
                                            && d.AHU_STATUS
                                            && c.ISACTIVE
                                            && l.ISACTIVE
                                            && d.TANGGAL_AKTA.HasValue
                                            //&& !d.CERTIFICATE_STATUS
                                            && b.ISACTIVE && (d.NILAI_JAMINAN >= b.MIN_VALUE && d.NILAI_JAMINAN <= b.MAX_VALUE)
                                            && f.ISACTIVE

                                     select new DataAhuModels
                                     {
                                         ID = d.ID,
                                         CLIENT_CODE = d.CLIENT_CODE,
                                         CLIENT_NAME = l.CLIENT_NAME,
                                         CUSTOMER_CODE = d.CUSTOMER_CODE,
                                         CUSTOMER_NAME = c.CUSTOMER_NAME,
                                         CUSTOMER_NPWP = c.NPWP,
                                         BRANCH_CODE = d.BRANCH_CODE,
                                         BRANCH_NAME = br.BRANCH_CODE == null ? d.BRANCH_CODE : br.BRANCH_NAME,
                                         NO_VOUCHER = d.NO_VOUCHER,
                                         NOTARIS_CODE = d.NOTARIS_CODE,
                                         NOTARIS_NAME = n.NOTARIS_NAME,
                                         NOMOR_AKTA = d.NOMOR_AKTA,
                                         TANGGAL_AKTA = d.TANGGAL_AKTA,
                                         NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                         NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                         NILAI_JAMINAN = d.NILAI_JAMINAN,
                                         PRICE_PNBP = b.PRICE,
                                         FEE_COST = f.FEE_COST,
                                         TAX_COST = f.TAX_COST
                                     }).AsQueryable();

                    // 3. Apply user type filter
                    if (currentUser.UserType == ConstantaData.INTERNAL)
                    {
                        baseQuery = baseQuery.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
                    }

                    baseQuery = baseQuery
                                .Where(d => (!string.IsNullOrEmpty(d.NO_VOUCHER) && nomorVoucher.Count > 0 && nomorVoucher.Contains(d.NO_VOUCHER.Trim().ToUpper()))
                                  || ((!string.IsNullOrEmpty(d.NOMOR_AKTA) && nomorAktaSet.Count > 0 && nomorAktaSet.Contains(d.NOMOR_AKTA.Trim().ToUpper()))
                                  && (d.TANGGAL_AKTA.HasValue && tanggalAktaSet.Contains(d.TANGGAL_AKTA.Value.Date))
                                  && (!string.IsNullOrEmpty(d.NOTARIS_NAME) && notarisNameSet.Count > 0 && notarisNameSet.Contains(d.NOTARIS_NAME.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.NAMA_PEMBERIFIDUSIA) && namaPemberiFidusiaSet.Count > 0 && namaPemberiFidusiaSet.Contains(d.NAMA_PEMBERIFIDUSIA.Trim().ToUpper()))
                                  && (!string.IsNullOrEmpty(d.CUSTOMER_NAME) && namaPenerimaFidusiaSet.Count > 0 && namaPenerimaFidusiaSet.Contains(d.CUSTOMER_NAME.Trim().ToUpper()))
                                  //&& (!string.IsNullOrEmpty(d.CLIENT_CODE) && clientCodeSet.Count > 0 && clientCodeSet.Contains(d.CLIENT_CODE.Trim().ToUpper()))
                                  )
                                );

                    // 4. Execute query and filter in memory
                    var allDataDocList = await baseQuery.AsNoTracking().ToListAsync();

                    //Join all
                    var joinedResults = param
                                        .SelectMany(p => allDataDocList
                                            .Where(r =>
                                                (p.NO_VOUCHER?.Trim() == r.NO_VOUCHER?.Trim()) ||
                                                (
                                                    p.NOMOR_AKTA?.Trim() == r.NOMOR_AKTA?.Trim() &&
                                                    (p.TANGGAL_AKTA.HasValue && r.TANGGAL_AKTA.HasValue && p.TANGGAL_AKTA.Value.Date == r.TANGGAL_AKTA.Value.Date) &&
                                                    p.NOTARIS_NAME?.Trim().ToUpper() == r.NOTARIS_NAME?.Trim().ToUpper() &&
                                                    p.NAMA_PEMBERIFIDUSIA?.Trim().ToUpper() == r.NAMA_PEMBERIFIDUSIA?.Trim() &&
                                                    p.NAMA_PENERIMAFIDUSIA?.Trim().ToUpper() == r.CUSTOMER_NAME?.Trim().ToUpper()
                                                )
                                            )
                                            .DefaultIfEmpty(),
                                            (p, r) => new CertificateModel
                                            {
                                                ID = r?.ID ?? 0,
                                                CLIENT_CODE = r?.CLIENT_CODE ?? p.CLIENT_CODE,
                                                CUSTOMER_CODE = r?.CUSTOMER_CODE ?? p.CUSTOMER_CODE,
                                                BRANCH_CODE = r?.BRANCH_CODE ?? p.BRANCH_CODE,
                                                BRANCH_NAME = r?.BRANCH_NAME ?? p.BRANCH_NAME,
                                                NOMOR_AKTA = r?.NOMOR_AKTA ?? p.NOMOR_AKTA,
                                                TANGGAL_AKTA = r?.TANGGAL_AKTA ?? p.TANGGAL_AKTA,
                                                NOTARIS_CODE = r?.NOTARIS_CODE ?? p.NOTARIS_CODE,
                                                NOTARIS_NAME = r?.NOTARIS_NAME ?? p.NOTARIS_NAME,
                                                NAMA_PEMBERIFIDUSIA = r?.NAMA_PEMBERIFIDUSIA ?? p.NAMA_PEMBERIFIDUSIA,
                                                NPWP_PEMBERIFIDUSIA = r?.NPWP_PEMBERIFIDUSIA ?? p.NPWP_PEMBERIFIDUSIA,
                                                NAMA_PENERIMAFIDUSIA = r?.CUSTOMER_NAME ?? p.NAMA_PENERIMAFIDUSIA,
                                                NPWP_PENERIMAFIDUSIA = r?.CUSTOMER_NPWP ?? p.NPWP_PENERIMAFIDUSIA,
                                                CODE_PENERIMAFIDUSIA = r?.CUSTOMER_CODE ?? p.CODE_PENERIMAFIDUSIA,
                                                NILAI_JAMINAN = r?.NILAI_JAMINAN ?? p.NILAI_JAMINAN,
                                                PRICE_PNBP = r?.PRICE_PNBP ?? p.PRICE_PNBP,
                                                FEE_COST = r?.FEE_COST ?? p.FEE_COST,
                                                TAX_COST = r?.TAX_COST ?? p.TAX_COST,
                                                NO_VOUCHER = p.NO_VOUCHER ?? r?.NO_VOUCHER,
                                                NO_SERTIFIKAT = p.NO_SERTIFIKAT,
                                                NO_REGISTRASI = p.NO_REGISTRASI,
                                                NO_PEMBIAYAAN = p.NO_PEMBIAYAAN,
                                                StatusCertificate = p.StatusCertificate,
                                                NameFileCertificate = p.NameFileCertificate,
                                                TypeFidusia = p.TypeFidusia,
                                                WILAYAH = p.WILAYAH,
                                                WaktuDaftar = p.WaktuDaftar,
                                            })
                                        .ToList();

                    

                    result.AddRange(joinedResults);
                }
                var response = new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel(ResponseCode.Error, "Error", 0, ex.Message);
            }
        }





        //SP

        public async Task<List<ColumnNameModel>> GetDocumentsColumnName()
        {
            return await _dbContext.Set<ColumnNameModel>()
                    .FromSqlRaw("SELECT * FROM sp_get_documents_column_name()")
                    .ToListAsync();
            //return await _dbContext.Set<ColumnNameModel>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetDocumentsColumnName]")
            //    .ToListAsync();
        }

        public async Task<DataTable> GetDocumentsCustomer(string userId, string clientCode, string customerCode, int startTake, int endTake, string? searchColumn = null, string? searchValue = null)
        {

            var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_get_documents_customer(@UserId, @ClientCode, @CustomerCode, @StartTake, @EndTake, @SearchColumn, @SearchValue)",parameters);

            //var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_GetDocumentsCustomer]", parameters);

        }

        public async Task<int> GetDocumentsCustomerTotal(string userId, string clientCode, string customerCode, string? searchColumn = null, string? searchValue = null)
        {

            var parameters = DbParameterHelper.CreateTotalParameters(userId, clientCode, customerCode, searchColumn, searchValue);

            var result = await _dbContext.Set<ResponseTotal>()
                .FromSqlRaw("SELECT * FROM sp_get_documents_customer_total(@UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return result?.Total ?? 0;

            //var parameters = DbParameterHelper.CreateTotalParameters(userId, clientCode, customerCode, searchColumn, searchValue);
            //var result = await _dbContext.Set<ResponseTotal>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetDocumentsCustomerTotal] @UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();

            //return result?.Total ?? 0;
        }

        public async Task<DataTable> GetDocumentsClient(string userId, string clientCode, string customerCode, int startTake, int endTake, string? searchColumn = null, string? searchValue = null)
        {
            var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_get_documents_client(@UserId, @ClientCode, @CustomerCode, @StartTake, @EndTake, @SearchColumn, @SearchValue)", parameters);

            //var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_GetDocumentsClient]", parameters);
        }

        public async Task<int> GetDocumentsClientTotal(string userId, string clientCode, string customerCode, string? searchColumn = null, string? searchValue = null)
        {
            var parameters = DbParameterHelper.CreateTotalParameters(userId, clientCode, customerCode, searchColumn, searchValue);
            var result = await _dbContext.Set<ResponseTotal>()
                .FromSqlRaw("SELECT * FROM sp_get_documents_client_total(@UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return result?.Total ?? 0;

            //var result = await _dbContext.Set<ResponseTotal>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetDocumentsClientTotal] @UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();

            //return result?.Total ?? 0;
        }       

        public async Task<DataTable> ExportDocumentsClient(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {
            var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_exp_documents_client(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters);

            //var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_ExpDocumentsClient]", parameters);
        }

        public async Task<DataTable> ExportDocumentsCustomer(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {

            var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_exp_documents_customer(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters);
            //var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_ExpDocumentsCustomer]", parameters);
        }

        public async Task<PriceDataDocument?> GetInvoiceCustomer(string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {
            var parameters = DbParameterHelper.CreateParameters(clientCode, customerCode, startDate, endDate);
            return await _dbContext.Set<PriceDataDocument>()
              .FromSqlRaw("SELECT * FROM sp_get_invoice_customer(@ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters)
              .AsNoTracking()
              .FirstOrDefaultAsync();
            //return await _dbContext.Set<PriceDataDocument>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetInvoiceCustomer] @ClientCode, @CustomerCode, @StartDate, @EndDate", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
        }

        
    }
}
