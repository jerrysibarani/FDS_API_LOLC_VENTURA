using API.Data.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using API.Helpers;
using API.IServices;
using API.Models;
using API.Models.Views;
using AutoMapper;
using API.Models.Params;
using API.Data.Entities;
using API.Model;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class DocumentService(
        ICoreService coreService,
        AppDbContext DBContext,
        IMapper mapper,
        IOptions<MySettings> options
    ) : IDocumentService
    {

        private readonly IMapper _mapper = mapper;
        private readonly ICoreService _coreService = coreService;
        private readonly AppDbContext _dbContext = DBContext;
        private readonly MySettings _settings = options.Value;

        private int batchSize { get; set; }

        private int GetBatchSizeFromConfig()
        {
            var value = _settings.BatchList!;
            return value > 0 ? value : 250;
        }


        public async Task<bool> PutDocument(Principal UserCurrent, int Id, DataModels Models, CancellationToken cancellationToken = default)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(Id) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                return false;
            }
            else
            {
                _mapper.Map(Models, result);
                result.MODIFIED_BY = UserCurrent.Email;
                result.MODIFIED_DATE = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(Models.BRANCH_CODE) && Models.BRANCH_CODE.Length > 5)
                {
                    result.BRANCH_CODE = Models.BRANCH_NAME;
                }
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
                return true;
            }
        }


        public async Task<bool> UpdateDocument(Principal UserCurrent, DataModels Models, CancellationToken cancellationToken = default)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(Models.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                return false;
            }
            else
            {
                _mapper.Map(Models, result);
                result.MODIFIED_BY = UserCurrent.Email;
                result.MODIFIED_DATE = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(Models.BRANCH_CODE) && Models.BRANCH_CODE.Length > 5)
                {
                    result.BRANCH_CODE = Models.BRANCH_NAME;
                }
                await _dbContext.SaveChangesAsync(cancellationToken); 
                _dbContext.ChangeTracker.Clear();
                return true;

            }
        }

        public async Task<bool> DeleteDocument(Principal UserCurrent, int Id, CancellationToken cancellationToken = default)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(Id) && x.DELETED_STATUS.Equals(false)).AsQueryable();
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }
            var result = await joinedResults.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                return false;
            }
            else
            {
                result.DELETED_BY = UserCurrent.Email;
                result.DELETED_DATE = DateTime.UtcNow;
                result.DELETED_STATUS = true;
                _dbContext.Documents.Update(result);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
                return true;
            }
        }

        public async Task<bool> StatusAhuDocument(Principal UserCurrent, ParamStatus Param, CancellationToken cancellationToken = default)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(Param.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();

            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            var result = await joinedResults.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(Param.NOTARIS_CODE) && string.IsNullOrEmpty(result.NOTARIS_CODE))
                {
                    result.NOTARIS_CODE = Param.NOTARIS_CODE;
                }

                if (!string.IsNullOrEmpty(Param.NO_VOUCHER))
                {
                    result.NO_VOUCHER = Param.NO_VOUCHER;
                }

                if (!string.IsNullOrEmpty(Param.MESSAGES) && Param.MESSAGES.Contains("Terdaftar"))
                {
                    result.MESSAGES = Param.MESSAGES;
                }
                else
                {
                    result.AHU_BY = UserCurrent.Email;
                    result.AHU_DATE = DateTime.UtcNow;
                    result.AHU_STATUS = true;
                    result.IS_DUPLICATE = false;
                }
                _dbContext.Documents.Update(result);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
                return true;
            }
        }

        public async Task<bool> UpdateNotarisDocument(Principal UserCurrent, ParamNotaris Param, CancellationToken cancellationToken = default)
        {
            var joinedResults = _dbContext.Documents.Where(x => x.ID.Equals(Param.ID) && x.DELETED_STATUS.Equals(false)).AsQueryable();

            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                joinedResults = joinedResults.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            var result = await joinedResults.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return false;
            }
            else
            {
                if (!String.IsNullOrEmpty(Param.NOTARIS_CODE))
                {
                    result.NOTARIS_CODE = Param.NOTARIS_CODE;
                    result.MODIFIED_BY = UserCurrent.Email;
                    result.MODIFIED_DATE = DateTime.UtcNow;
                    _dbContext.Documents.Update(result);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();
                }
                return true;
            }
        }


        public async Task<ResponseModel> GenerateCertificates(Principal UserCurrent, List<CertificateModel> Param, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Ambil batch number (lebih ringkas)
                int lastBatch = await _dbContext.HistoriesCertificates.Select(x => (int?)x.BATCH_NUMBER).MaxAsync(cancellationToken) ?? 0;
                int batchNumber = lastBatch + 1;

                // 2. Identify unique codes to look up (prevents loading the whole Customer table)
                var requestedCodes = Param.Select(x => x.CUSTOMER_CODE).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

                // 3. Optimized Lookup: Fetch only needed customers and build a dictionary
                var customerLookup = await _dbContext.Customers
                                            .Where(x => x.ISACTIVE && x.CUSTOMER_CODE != null &&
                                                       (requestedCodes.Contains(x.CUSTOMER_CODE) || requestedCodes.Contains(x.CODE_ALIAS)))
                                            .AsNoTracking()
                                            .ToDictionaryAsync(
                                                x => x.CUSTOMER_CODE!, // The ! tells the compiler this won't be null
                                                x => x,
                                                cancellationToken);

                // 4. Map and Process
                var certificates = _mapper.Map<List<HISTORY_CERTIFICATE>>(Param);
                var now = DateTime.UtcNow;
                foreach (var cert in certificates)
                {
                    if (!string.IsNullOrEmpty(cert.CUSTOMER_CODE))
                    {
                        // Check main code first, then fallback to alias via search if dictionary key isn't a direct hit
                        var customer = customerLookup.GetValueOrDefault(cert.CUSTOMER_CODE) ?? customerLookup.Values.FirstOrDefault(c => c.CODE_ALIAS == cert.CUSTOMER_CODE);

                        if (customer != null)
                        {
                            cert.NAMA_PENERIMAFIDUSIA = customer.CUSTOMER_NAME;
                            cert.CUSTOMER_CODE = customer.CUSTOMER_CODE;
                        }
                    }

                    cert.BATCH_NUMBER = batchNumber;
                    cert.CREATED_BY = UserCurrent.Email;
                    cert.CREATED_DATE = now;
                }


                // Simpan history awal agar memiliki ID
                await _dbContext.HistoriesCertificates.AddRangeAsync(certificates, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
                var lsBatch = await _dbContext.HistoriesCertificates.Where(x => x.BATCH_NUMBER == batchNumber).AsNoTracking().ToListAsync(cancellationToken);

                // 3. OPTIMASI: Ambil data Document & Customer sekaligus (Join Manual)
                // Kita filter dlu seminimal mungkin agar tidak menarik seluruh isi database
                var joinedData = await (from d in _dbContext.Documents
                                        join p in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                                        join br in _dbContext.Branches on d.BRANCH_CODE equals br.BRANCH_CODE into bcDefault
                                        from br in bcDefault.DefaultIfEmpty()

                                        where !d.DELETED_STATUS && d.AHU_STATUS && d.IMPORT_STATUS  && d.INSERT_DATE.HasValue && p.ISACTIVE
                                        && !d.CERTIFICATE_STATUS
                                        select new {
                                            Document = d, 
                                            CustomerName = p.CUSTOMER_NAME,
                                            BRANCH_NAME = br.BRANCH_CODE == null ? d.BRANCH_CODE : br.BRANCH_NAME,
                                        })
                                        .AsNoTracking() // Jika hanya untuk matching, gunakan NoTracking
                                        .ToListAsync(cancellationToken);

                bool hasUpdates = false;
               
                // 4. Proses Matching di Memori
                foreach (var bt in lsBatch)
                {
                    // Cari di list hasil join tadi
                    var match = joinedData.FirstOrDefault(x =>
                        x.Document.TANGGAL_AKTA == bt.TANGGAL_AKTA &&
                        x.Document.NOMOR_AKTA?.Trim() == bt.NOMOR_AKTA?.Trim() &&
                        x.Document.NAMA_PEMBERIFIDUSIA?.Trim().ToUpper() == bt.NAMA_PEMBERIFIDUSIA?.Trim().ToUpper() &&
                        x.Document.NOTARIS_CODE?.Trim().ToUpper().Contains(bt.NOTARIS_NAME?.Trim().ToUpper() ?? "") == true &&
                        x.CustomerName?.Trim().ToUpper().Contains(bt.NAMA_PENERIMAFIDUSIA?.Trim().ToUpper() ?? "") == true
                    );

                    if (match != null)
                    {
                        var dataDoc = match.Document;

                        // Karena tadi AsNoTracking, kita perlu 'Attach' kembali atau Update langsung jika menggunakan ChangeTracker
                        _dbContext.Documents.Attach(dataDoc);

                        dataDoc.NO_REGISTRASI = bt.NO_REGISTRASI;
                        dataDoc.NO_VOUCHER = bt.NO_VOUCHER;
                        dataDoc.NO_PEMBIAYAAN = bt.NO_PEMBIAYAAN;
                        dataDoc.NO_SERTIFIKAT = bt.NO_SERTIFIKAT;
                        dataDoc.TGL_SERTIFIKAT = bt.WAKTU_DAFTAR;
                        dataDoc.CERTIFICATE_DATE = bt.CREATED_DATE;
                        dataDoc.CERTIFICATE_STATUS = bt.STATUS_CERTIFICATE;
                        dataDoc.CERTIFICATE_BY = bt.CREATED_BY;

                        _dbContext.Documents.Update(dataDoc);


                        _dbContext.HistoriesCertificates.Attach(bt);
                        bt.DOCUMENT_ID = dataDoc.ID;
                        bt.ISMATCH = true;
                        bt.BRANCH_CODE = match.BRANCH_NAME;
                        bt.BRANCH_NAME = match.BRANCH_NAME;
                        _dbContext.HistoriesCertificates.Update(bt);
                        hasUpdates = true;
                    }
                }

                if (hasUpdates)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();
                    lsBatch = await _dbContext.HistoriesCertificates.Where(x => x.BATCH_NUMBER == batchNumber).AsNoTracking().ToListAsync(cancellationToken);
                }

                var result = _mapper.Map<List<CertificateModel>>(lsBatch);
                //return new ResponseModel(ResponseCode.OK, "Success", lsBatch.Count, lsBatch);
                return new ResponseModel(ResponseCode.OK, "Success", result.Count, result);
            }
            catch (Exception ex)
            {
                //return new ResponseModel(ResponseCode.OK, "Error"+ex.Message.ToString(), Param.Count, Param);
                return new ResponseModel(ResponseCode.Error, "Error", 0, ex.Message);
            }



        }



        //OLD

        public async Task<ResponseModel> UpdateCertificates(Principal UserCurrent, List<CertificateModel> Param, CancellationToken cancellationToken = default)
        {
            try
            {

                var result = new List<CertificateModel>();
                var allUpdatedDocuments = new List<DOCUMENTS>();

                //for (int i = 0; i < Param.Count; i += batchSize)
                //{
                //    var batch = Param.Skip(i).Take(batchSize).ToList();

                    // 1. Prepare HashSets for filtering
                    var nomorAktaSet = Param.Select(p => p.NOMOR_AKTA!).ToHashSet();
                    var tanggalAktaSet = Param.Where(p => p.TANGGAL_AKTA.HasValue).Select(p => p.TANGGAL_AKTA!.Value.Date).ToHashSet();
                    var namaPemberiFidusiaSet = Param.Select(p => p.NAMA_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var npwpPemberiFidusiaSet = batch.Where(p => !string.IsNullOrEmpty(p.NPWP_PEMBERIFIDUSIA)).Select(p => p.NPWP_PEMBERIFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    var notarisNameSet = Param.Where(p => !string.IsNullOrWhiteSpace(p.NOTARIS_NAME)).Select(p => p.NOTARIS_NAME!.ToString().Trim().ToUpper()).ToHashSet();
                    var namaPenerimaFidusiaSet = Param.Where(p => !string.IsNullOrWhiteSpace(p.NAMA_PENERIMAFIDUSIA)).Select(p => p.NAMA_PENERIMAFIDUSIA!.ToString().Trim().ToUpper()).ToHashSet();
                    //var clientCodeSet = batch.Where(p => !string.IsNullOrWhiteSpace(p.CLIENT_CODE)).Select(p => p.CLIENT_CODE!.ToString().Trim().ToUpper()).ToHashSet();
                    var nomorVoucher = Param.Select(p => p.NO_VOUCHER!.ToString().Trim().ToUpper()).ToHashSet();

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
                    if (UserCurrent.UserType == ConstantaData.INTERNAL)
                    {
                        baseQuery = baseQuery.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
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
                    var allDataDocList = await baseQuery.AsNoTracking().ToListAsync(cancellationToken);

                    //Join all
                    var joinedResults = Param
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
                                                DOCUMENT_ID = r?.ID ?? 0,
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
                                                STATUS_CERTIFICATE = p.STATUS_CERTIFICATE,
                                                FILENAME_CERTIFICATE = p.FILENAME_CERTIFICATE,
                                                TYPE_FIDUSIA = p.TYPE_FIDUSIA,
                                                WILAYAH = p.WILAYAH,
                                                WAKTU_DAFTAR = p.WAKTU_DAFTAR,
                                            })
                                        .ToList();

                    var listDoc = joinedResults.Where(x => x.DOCUMENT_ID > 0 && !string.IsNullOrEmpty(x.NO_VOUCHER)).ToList();
                    if (listDoc.Count > 0)
                    {
                        var idsToUpdate = listDoc.Select(x => x.DOCUMENT_ID).ToList();
                        var documentsToUpdate = await _dbContext.Documents.Where(x => idsToUpdate.Contains(x.ID) && !x.DELETED_STATUS && !string.IsNullOrEmpty(x.NOMOR_AKTA)).ToListAsync();
                        if (documentsToUpdate.Any())
                        {
                            foreach (var doc in documentsToUpdate)
                            {
                                var match = listDoc.FirstOrDefault(x =>
                                            x.DOCUMENT_ID == doc.ID &&
                                            (x.NO_VOUCHER == doc.NO_VOUCHER ||
                                            (x.CLIENT_CODE == doc.CLIENT_CODE &&
                                              x.CUSTOMER_CODE == doc.CUSTOMER_CODE &&
                                              x.NOTARIS_CODE == doc.NOTARIS_CODE &&
                                              x.NOMOR_AKTA == doc.NOMOR_AKTA &&
                                              x.TANGGAL_AKTA?.Date == doc.TANGGAL_AKTA?.Date
                                            )));

                                if (match != null)
                                {
                                    if (match.STATUS_CERTIFICATE)
                                    {
                                        doc.NO_SERTIFIKAT = match.NO_SERTIFIKAT;
                                        doc.TGL_SERTIFIKAT = Convert.ToDateTime(match.WAKTU_DAFTAR);
                                        doc.CERTIFICATE_STATUS = true;
                                        doc.CERTIFICATE_BY = UserCurrent.Email;
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
                //}

                // Update dan simpan semua dokumen sekaligus
                if (allUpdatedDocuments.Any())
                {
                    _dbContext.Documents.UpdateRange(allUpdatedDocuments);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();
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



        public async Task<IReadOnlyList<ColumnNameModel>> GetDocumentsColumnName(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ColumnNameModel>()
                    .FromSqlRaw("SELECT * FROM sp_get_documents_column_name()")
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            //return await _dbContext.Set<ColumnNameModel>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetDocumentsColumnName]")
            //    .ToListAsync();
        }

        public async Task<DataTable> GetDocumentsCustomer(string userId, string clientCode, string customerCode, int startTake, int endTake, string? searchColumn = null, string? searchValue = null, CancellationToken cancellationToken = default)
        {

            var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_get_documents_customer(@UserId, @ClientCode, @CustomerCode, @StartTake, @EndTake, @SearchColumn, @SearchValue)",parameters, cancellationToken);

            //var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_GetDocumentsCustomer]", parameters);

        }

        public async Task<int> GetDocumentsCustomerTotal(string userId, string clientCode, string customerCode, string? searchColumn = null, string? searchValue = null, CancellationToken cancellationToken = default)
        {

            var parameters = DbParameterHelper.CreateTotalParameters(userId, clientCode, customerCode, searchColumn, searchValue);

            var result = await _dbContext.Set<ResponseTotal>()
                .FromSqlRaw("SELECT * FROM sp_get_documents_customer_total(@UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            return result?.Total ?? 0;

            //var parameters = DbParameterHelper.CreateTotalParameters(userId, clientCode, customerCode, searchColumn, searchValue);
            //var result = await _dbContext.Set<ResponseTotal>()
            //    .FromSqlRaw("EXEC [dbo].[sp_GetDocumentsCustomerTotal] @UserId, @ClientCode, @CustomerCode, @SearchColumn, @SearchValue", parameters)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();

            //return result?.Total ?? 0;
        }

        public async Task<DataTable> GetDocumentsClient(string userId, string clientCode, string customerCode, int startTake, int endTake, string? searchColumn = null, string? searchValue = null, CancellationToken cancellationToken = default)
        {
            var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_get_documents_client(@UserId, @ClientCode, @CustomerCode, @StartTake, @EndTake, @SearchColumn, @SearchValue)", parameters);

            //var parameters = DbParameterHelper.CreatePagingParameters(userId, clientCode, customerCode, startTake, endTake, searchColumn, searchValue);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_GetDocumentsClient]", parameters);
        }

        public async Task<int> GetDocumentsClientTotal(string userId, string clientCode, string customerCode, string? searchColumn = null, string? searchValue = null, CancellationToken cancellationToken = default)
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

        public async Task<DataTable> ExportDocumentsClient(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_exp_documents_client(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters);

            //var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_ExpDocumentsClient]", parameters);
        }

        public async Task<DataTable> ExportDocumentsCustomer(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {

            var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            return await _coreService.ExecSPToDataTable("SELECT * FROM sp_exp_documents_customer(@UserId, @ClientCode, @CustomerCode, @StartDate, @EndDate)", parameters);
            //var parameters = DbParameterHelper.CreateUserParameters(userId, clientCode, customerCode, startDate, endDate);
            //return await _coreService.ExecSPToDataTable("[dbo].[sp_ExpDocumentsCustomer]", parameters);
        }

        public async Task<PriceDataDocument?> GetInvoiceCustomer(string clientCode, string customerCode, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
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
