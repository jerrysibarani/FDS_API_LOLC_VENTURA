using API.Data;
using API.Data.Entities;
using API.Helpers;
using API.IServices;
using API.Model;
using API.Models;
using API.Models.Params;
using API.Models.Views;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API.Services
{
    public class DataService(
        AppDbContext DBContext
    ) : IDataService
    {
        private readonly AppDbContext _dbContext = DBContext;
        public async Task<ResponseModel> GetForAHU_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {

            var startDate = Param.InsertDate.ToDateTime(TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);
            //var notarisDefault = await _dbContext.Notaris
            //                    .AsNoTracking()
            //                    .Where(n =>
            //                        n.ISACTIVE &&
            //                        _dbContext.Customers.Any(c =>
            //                            c.CLIENT_CODE == n.CLIENT_CODE &&
            //                            c.ISACTIVE &&
            //                            c.CUSTOMER_CODE == Param.CustomerCode
            //                        ))
            //                    .FirstOrDefaultAsync();

            IQueryable <DataModels> query = from d in _dbContext.Documents.AsNoTracking()
                                              join p in _dbContext.Customers.AsNoTracking() on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                                              join c in _dbContext.Clients.AsNoTracking() on d.CLIENT_CODE equals c.CLIENT_CODE
                                              join t in _dbContext.Notaris.AsNoTracking() on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
                                              from t in ntrsDefault.DefaultIfEmpty()
                                              join b in _dbContext.Branches.AsNoTracking() on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
                                              from b in bcDefault.DefaultIfEmpty()
                                              where
                                                !d.DELETED_STATUS &&
                                                d.IMPORT_STATUS &&
                                                p.ISACTIVE &&
                                                c.ISACTIVE &&
                                                (
                                                  (d.INSERT_DATE.HasValue && d.INSERT_DATE >= startDate && d.INSERT_DATE < endDate) ||
                                                  (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE >= startDate && d.DUPLICATED_DATE < endDate)
                                                )
                                            select new DataModels
                                              {
                                                  ID = d.ID,
                                                  CLIENT_CODE = d.CLIENT_CODE,
                                                  CLIENT_NAME = c.CLIENT_NAME,
                                                  CUSTOMER_CODE = d.CUSTOMER_CODE,
                                                  CUSTOMER_NAME = p.CUSTOMER_NAME,
                                                  TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                                                  BRANCH_CODE = d.BRANCH_CODE,
                                                  BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                                                  NOTARIS_CODE = d.NOTARIS_CODE,
                                                  NOTARIS_NAME = t.NOTARIS_NAME,
                                                  NO_REGISTRASI = d.NO_REGISTRASI,
                                                  NO_VOUCHER = d.NO_VOUCHER,
                                                  NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                                                  NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                                                  TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                                                  JAM_MINUTA = d.JAM_MINUTA,
                                                  TANGGAL_AKTA = d.TANGGAL_AKTA,
                                                  NOMOR_AKTA = d.NOMOR_AKTA,
                                                  TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                                                  ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                                                  NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                                  NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                                  JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                                                  MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                                                  TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                                                  TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                                                  PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                                                  ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                                                  RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                                                  RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                                                  KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                                                  KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                                                  KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                                                  PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                                                  POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                                                  HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                                                  ID_PASANGAN = d.ID_PASANGAN,
                                                  NAMA_PASANGAN = d.NAMA_PASANGAN,
                                                  JK_PASANGAN = d.JK_PASANGAN,
                                                  MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                                                  TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                                                  TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                                                  ID_DEBITUR = d.ID_DEBITUR,
                                                  NAMA_DEBITUR = d.NAMA_DEBITUR,
                                                  JK_DEBITUR = d.JK_DEBITUR,
                                                  MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                                                  TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                                                  TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                                                  ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                                                  RT_DEBITUR = d.RT_DEBITUR,
                                                  RW_DEBITUR = d.RW_DEBITUR,
                                                  KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                                                  KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                                                  KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                                                  PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                                                  POS_DEBITUR = d.POS_DEBITUR,
                                                  HP_DEBITUR = d.HP_DEBITUR,
                                                  TANGGAL_ORDER = d.TANGGAL_ORDER,
                                                  TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                                                  NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                                                  HUTANG_POKOK = d.HUTANG_POKOK,
                                                  NILAI_JAMINAN = d.NILAI_JAMINAN,
                                                  NILAI_BARANG = d.NILAI_BARANG,
                                                  CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                                                  JENIS_OBJECT = d.JENIS_OBJECT,
                                                  MODEL = d.MODEL,
                                                  MERK = d.MERK,
                                                  TIPE = d.TIPE,
                                                  TAHUN = d.TAHUN,
                                                  WARNA = d.WARNA,
                                                  NOMOR_RANGKA = d.NOMOR_RANGKA,
                                                  NOMOR_MESIN = d.NOMOR_MESIN,
                                                  NOMOR_POLISI = d.NOMOR_POLISI,
                                                  NOMOR_BPKB = d.NOMOR_BPKB,
                                                  NAMA_BPKB = d.NAMA_BPKB,
                                                  PEMILIK_BPKB = d.PEMILIK_BPKB,
                                                  TENOR = d.TENOR,
                                                  TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                                                  TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                                                  TYPE_PRODUK = d.TYPE_PRODUK,
                                                  WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                                                  NAMA_KWITANSI = d.NAMA_KWITANSI,
                                                  INSERT_DATE = d.INSERT_DATE,
                                                  AHU_BY = d.AHU_BY,
                                                  AHU_DATE = d.AHU_DATE,
                                                  AHU_STATUS = d.AHU_STATUS,
                                                  USER_BY = d.USER_BY,
                                                  CERTIFICATE_BY = d.CERTIFICATE_BY,
                                                  CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                                                  CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                                                  CREATED_BY = d.CREATED_BY,
                                                  CREATED_DATE = d.CREATED_DATE,
                                                  MODIFIED_BY = d.MODIFIED_BY,
                                                  MODIFIED_DATE = d.MODIFIED_DATE,
                                                  DELETED_STATUS = d.DELETED_STATUS,
                                                  DELETED_DATE = d.DELETED_DATE,
                                                  DELETED_BY = d.DELETED_BY,
                                                  MESSAGES = d.MESSAGES,
                                                  IS_DUPLICATE = d.IS_DUPLICATE,
                                                  DUPLICATED_DATE = d.DUPLICATED_DATE,

                                                  TIPE_CUSTOMER = p.TIPE,
                                                  SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                                                  TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                                                  JENIS_CUSTOMER = p.JENIS,
                                                  NPWP_CUSTOMER = p.NPWP,
                                                  NIK_CUSTOMER = p.NIK,
                                                  SK_CUSTOMER = p.SK,
                                                  NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                                                  TELP_CUSTOMER = p.TELP,
                                                  EMAIL_CUSTOMER = p.EMAIL,
                                                  KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                                                  ALAMAT_CUSTOMER = p.ALAMAT,
                                                  RT_CUSTOMER = p.RT,
                                                  RW_CUSTOMER = p.RW,
                                                  PROVINSI_CUSTOMER = p.PROVINSI,
                                                  KOTA_CUSTOMER = p.KOTA,
                                                  KECAMATAN_CUSTOMER = p.KECAMATAN,
                                                  KELURAHAN_CUSTOMER = p.KELURAHAN,
                                                  POS_CUSTOMER = p.POS,
                                                  ISBRANCH = p.ISBRANCH,
                                                  CODE_ALIAS = p.CODE_ALIAS
                                            };

            // ===============================
            // KEYSET CONDITION
            // ===============================
            //if (Param.StartRecord > 0)
            //{
            //    query = query.Where(x => x.ID > Param.StartRecord);
            //}

            // ===============================
            // User Filter
            // ===============================
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }


            // ===============================
            // Search (PostgreSQL Friendly)
            // ===============================
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                var search = Param.ParamSearch.Trim();
                if (bool.TryParse(search, out bool ahuStatus))
                {
                    query = query.Where(x => x.AHU_STATUS == ahuStatus);
                }
                else
                {
                    query = query.Where(x =>
                        (x.CLIENT_CODE != null && EF.Functions.ILike(x.CLIENT_CODE, $"%{search}%")) ||
                        (x.CLIENT_NAME != null && EF.Functions.ILike(x.CLIENT_NAME, $"%{search}%")) ||
                        (x.CUSTOMER_CODE != null && EF.Functions.ILike(x.CUSTOMER_CODE, $"%{search}%")) ||
                        (x.CUSTOMER_NAME != null && EF.Functions.ILike(x.CUSTOMER_NAME, $"%{search}%")) ||
                        (x.BRANCH_CODE != null && EF.Functions.ILike(x.BRANCH_CODE, $"%{search}%")) ||
                        (x.BRANCH_NAME != null && EF.Functions.ILike(x.BRANCH_NAME, $"%{search}%")) ||
                        (x.NOTARIS_CODE != null && EF.Functions.ILike(x.NOTARIS_CODE, $"%{search}%")) ||
                        (x.NOTARIS_NAME != null && EF.Functions.ILike(x.NOTARIS_NAME, $"%{search}%")) ||
                        //(x.NO_REGISTRASI != null && EF.Functions.ILike(x.NO_REGISTRASI, $"%{search}%")) ||
                        //(x.NO_VOUCHER != null && EF.Functions.ILike(x.NO_VOUCHER, $"%{search}%")) ||
                        //(x.NO_PEMBIAYAAN != null && EF.Functions.ILike(x.NO_PEMBIAYAAN, $"%{search}%")) ||
                        //(x.NO_SERTIFIKAT != null && EF.Functions.ILike(x.NO_SERTIFIKAT, $"%{search}%")) ||
                        (x.NOMOR_AKTA != null && EF.Functions.ILike(x.NOMOR_AKTA, $"%{search}%")) ||
                        (x.ID_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.ID_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NPWP_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NPWP_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NAMA_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PASANGAN != null && EF.Functions.ILike(x.NAMA_PASANGAN, $"%{search}%")) ||
                        (x.NAMA_DEBITUR != null && EF.Functions.ILike(x.NAMA_DEBITUR, $"%{search}%")) ||
                        (x.NOMOR_KONTRAK != null && EF.Functions.ILike(x.NOMOR_KONTRAK, $"%{search}%")) ||
                        (x.JENIS_OBJECT != null && EF.Functions.ILike(x.JENIS_OBJECT, $"%{search}%")) ||
                        (x.NOMOR_RANGKA != null && EF.Functions.ILike(x.NOMOR_RANGKA, $"%{search}%")) ||
                        (x.NOMOR_MESIN != null && EF.Functions.ILike(x.NOMOR_MESIN, $"%{search}%")) ||
                        (x.NOMOR_POLISI != null && EF.Functions.ILike(x.NOMOR_POLISI, $"%{search}%")) ||
                        (x.NOMOR_BPKB != null && EF.Functions.ILike(x.NOMOR_BPKB, $"%{search}%")) ||
                        (x.NAMA_KWITANSI != null && EF.Functions.ILike(x.NAMA_KWITANSI, $"%{search}%")) 
                    );

                    if (DateTime.TryParseExact(search,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime parsedDate))
                    {
                        var ds = parsedDate.Date;
                        var de = ds.AddDays(1);
                        query = query.Where(x =>
                                (x.INSERT_DATE >= ds && x.INSERT_DATE < de) ||
                                (x.TANGGAL_AKTA.HasValue && x.TANGGAL_AKTA >= ds && x.TANGGAL_AKTA < de) ||
                                (x.TANGGAL_ORDER.HasValue && x.TANGGAL_ORDER >= ds && x.TANGGAL_ORDER < de) ||
                                (x.TANGGAL_KONTRAK.HasValue && x.TANGGAL_KONTRAK >= ds && x.TANGGAL_KONTRAK < de) ||
                                (x.TGL_SERTIFIKAT.HasValue && x.TGL_SERTIFIKAT >= ds && x.TGL_SERTIFIKAT < de)
                        );
                    }
                }
            }

            // ===============================
            // ORDER + LIMIT
            // ===============================

            //var data = await query
            //    .OrderBy(x => x.ID)
            //    .Take(Param.PageSize)
            //    .AsNoTracking()
            //    .ToListAsync(cancellationToken);

            //long? nextCursor = data.LastOrDefault()?.ID;

            //return new ResponseModel(
            //    ResponseCode.OK,
            //    "Success",
            //    data.Count,
            //    new
            //    {
            //        Items = data,
            //        NextCursor = nextCursor
            //    }
            //);

            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .OrderBy(x => x.ID)
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return new ResponseModel(ResponseCode.OK, "Success", total, dataList);

        }

        public async Task<ResponseModel> GetForCertificate_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {

            var startDate = Param.InsertDate.ToDateTime(TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);
            IQueryable<DataModels> query = from d in _dbContext.Documents.AsNoTracking()
                                           join p in _dbContext.Customers.AsNoTracking() on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                                           join c in _dbContext.Clients.AsNoTracking() on d.CLIENT_CODE equals c.CLIENT_CODE
                                           join t in _dbContext.Notaris.AsNoTracking() on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
                                           from t in ntrsDefault.DefaultIfEmpty()
                                           join b in _dbContext.Branches.AsNoTracking() on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
                                           from b in bcDefault.DefaultIfEmpty()
                                           where
                                                !d.DELETED_STATUS &&
                                                d.AHU_STATUS &&
                                                d.IMPORT_STATUS &&
                                                p.ISACTIVE &&
                                                c.ISACTIVE &&
                                                ( 
                                                  (d.INSERT_DATE.HasValue && d.INSERT_DATE >= startDate && d.INSERT_DATE < endDate) || 
                                                  (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE >= startDate && d.DUPLICATED_DATE < endDate) 
                                                )
                                           select new DataModels
                                           {
                                               ID = d.ID,
                                               CLIENT_CODE = d.CLIENT_CODE,
                                               CLIENT_NAME = c.CLIENT_NAME,
                                               CUSTOMER_CODE = d.CUSTOMER_CODE,
                                               CUSTOMER_NAME = p.CUSTOMER_NAME,
                                               TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                                               BRANCH_CODE = d.BRANCH_CODE,
                                               BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                                               NOTARIS_CODE = d.NOTARIS_CODE,
                                               NOTARIS_NAME = t.NOTARIS_NAME,
                                               NO_REGISTRASI = d.NO_REGISTRASI,
                                               NO_VOUCHER = d.NO_VOUCHER,
                                               NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                                               NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                                               TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                                               JAM_MINUTA = d.JAM_MINUTA,
                                               TANGGAL_AKTA = d.TANGGAL_AKTA,
                                               NOMOR_AKTA = d.NOMOR_AKTA,
                                               TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                                               ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                                               NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                               NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                               JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                                               MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                                               TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                                               TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                                               PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                                               ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                                               RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                                               RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                                               KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                                               KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                                               KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                                               PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                                               POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                                               HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                                               ID_PASANGAN = d.ID_PASANGAN,
                                               NAMA_PASANGAN = d.NAMA_PASANGAN,
                                               JK_PASANGAN = d.JK_PASANGAN,
                                               MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                                               TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                                               TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                                               ID_DEBITUR = d.ID_DEBITUR,
                                               NAMA_DEBITUR = d.NAMA_DEBITUR,
                                               JK_DEBITUR = d.JK_DEBITUR,
                                               MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                                               TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                                               TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                                               ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                                               RT_DEBITUR = d.RT_DEBITUR,
                                               RW_DEBITUR = d.RW_DEBITUR,
                                               KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                                               KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                                               KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                                               PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                                               POS_DEBITUR = d.POS_DEBITUR,
                                               HP_DEBITUR = d.HP_DEBITUR,
                                               TANGGAL_ORDER = d.TANGGAL_ORDER,
                                               TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                                               NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                                               HUTANG_POKOK = d.HUTANG_POKOK,
                                               NILAI_JAMINAN = d.NILAI_JAMINAN,
                                               NILAI_BARANG = d.NILAI_BARANG,
                                               CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                                               JENIS_OBJECT = d.JENIS_OBJECT,
                                               MODEL = d.MODEL,
                                               MERK = d.MERK,
                                               TIPE = d.TIPE,
                                               TAHUN = d.TAHUN,
                                               WARNA = d.WARNA,
                                               NOMOR_RANGKA = d.NOMOR_RANGKA,
                                               NOMOR_MESIN = d.NOMOR_MESIN,
                                               NOMOR_POLISI = d.NOMOR_POLISI,
                                               NOMOR_BPKB = d.NOMOR_BPKB,
                                               NAMA_BPKB = d.NAMA_BPKB,
                                               PEMILIK_BPKB = d.PEMILIK_BPKB,
                                               TENOR = d.TENOR,
                                               TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                                               TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                                               TYPE_PRODUK = d.TYPE_PRODUK,
                                               WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                                               NAMA_KWITANSI = d.NAMA_KWITANSI,
                                               INSERT_DATE = d.INSERT_DATE,
                                               AHU_BY = d.AHU_BY,
                                               AHU_DATE = d.AHU_DATE,
                                               AHU_STATUS = d.AHU_STATUS,
                                               USER_BY = d.USER_BY,
                                               CERTIFICATE_BY = d.CERTIFICATE_BY,
                                               CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                                               CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                                               CREATED_BY = d.CREATED_BY,
                                               CREATED_DATE = d.CREATED_DATE,
                                               MODIFIED_BY = d.MODIFIED_BY,
                                               MODIFIED_DATE = d.MODIFIED_DATE,
                                               DELETED_STATUS = d.DELETED_STATUS,
                                               DELETED_DATE = d.DELETED_DATE,
                                               DELETED_BY = d.DELETED_BY,
                                               MESSAGES = d.MESSAGES,
                                               IS_DUPLICATE = d.IS_DUPLICATE,
                                               DUPLICATED_DATE = d.DUPLICATED_DATE,

                                               TIPE_CUSTOMER = p.TIPE,
                                               SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                                               TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                                               JENIS_CUSTOMER = p.JENIS,
                                               NPWP_CUSTOMER = p.NPWP,
                                               NIK_CUSTOMER = p.NIK,
                                               SK_CUSTOMER = p.SK,
                                               NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                                               TELP_CUSTOMER = p.TELP,
                                               EMAIL_CUSTOMER = p.EMAIL,
                                               KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                                               ALAMAT_CUSTOMER = p.ALAMAT,
                                               RT_CUSTOMER = p.RT,
                                               RW_CUSTOMER = p.RW,
                                               PROVINSI_CUSTOMER = p.PROVINSI,
                                               KOTA_CUSTOMER = p.KOTA,
                                               KECAMATAN_CUSTOMER = p.KECAMATAN,
                                               KELURAHAN_CUSTOMER = p.KELURAHAN,
                                               POS_CUSTOMER = p.POS,
                                               ISBRANCH = p.ISBRANCH,
                                               CODE_ALIAS = p.CODE_ALIAS
                                           };

            // ===============================
            // KEYSET CONDITION
            // ===============================
            //if (Param.StartRecord > 0)
            //{
            //    query = query.Where(x => x.ID > Param.StartRecord);
            //}

            // ===============================
            // User Filter
            // ===============================
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }


            // ===============================
            // Search (PostgreSQL Friendly)
            // ===============================
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                var search = Param.ParamSearch.Trim();
                if (bool.TryParse(search, out bool ahuStatus))
                {
                    query = query.Where(x => x.CERTIFICATE_STATUS == ahuStatus);
                }
                else
                {
                    query = query.Where(x =>
                        (x.CLIENT_CODE != null && EF.Functions.ILike(x.CLIENT_CODE, $"%{search}%")) ||
                        (x.CLIENT_NAME != null && EF.Functions.ILike(x.CLIENT_NAME, $"%{search}%")) ||
                        (x.CUSTOMER_CODE != null && EF.Functions.ILike(x.CUSTOMER_CODE, $"%{search}%")) ||
                        (x.CUSTOMER_NAME != null && EF.Functions.ILike(x.CUSTOMER_NAME, $"%{search}%")) ||
                        (x.BRANCH_CODE != null && EF.Functions.ILike(x.BRANCH_CODE, $"%{search}%")) ||
                        (x.BRANCH_NAME != null && EF.Functions.ILike(x.BRANCH_NAME, $"%{search}%")) ||
                        (x.NOTARIS_CODE != null && EF.Functions.ILike(x.NOTARIS_CODE, $"%{search}%")) ||
                        (x.NOTARIS_NAME != null && EF.Functions.ILike(x.NOTARIS_NAME, $"%{search}%")) ||
                        (x.NO_REGISTRASI != null && EF.Functions.ILike(x.NO_REGISTRASI, $"%{search}%")) ||
                        (x.NO_VOUCHER != null && EF.Functions.ILike(x.NO_VOUCHER, $"%{search}%")) ||
                        (x.NO_PEMBIAYAAN != null && EF.Functions.ILike(x.NO_PEMBIAYAAN, $"%{search}%")) ||
                        (x.NO_SERTIFIKAT != null && EF.Functions.ILike(x.NO_SERTIFIKAT, $"%{search}%")) ||
                        (x.NOMOR_AKTA != null && EF.Functions.ILike(x.NOMOR_AKTA, $"%{search}%")) ||
                        (x.ID_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.ID_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NPWP_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NPWP_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NAMA_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PASANGAN != null && EF.Functions.ILike(x.NAMA_PASANGAN, $"%{search}%")) ||
                        (x.NAMA_DEBITUR != null && EF.Functions.ILike(x.NAMA_DEBITUR, $"%{search}%")) ||
                        (x.NOMOR_KONTRAK != null && EF.Functions.ILike(x.NOMOR_KONTRAK, $"%{search}%")) ||
                        (x.JENIS_OBJECT != null && EF.Functions.ILike(x.JENIS_OBJECT, $"%{search}%")) ||
                        (x.NOMOR_RANGKA != null && EF.Functions.ILike(x.NOMOR_RANGKA, $"%{search}%")) ||
                        (x.NOMOR_MESIN != null && EF.Functions.ILike(x.NOMOR_MESIN, $"%{search}%")) ||
                        (x.NOMOR_POLISI != null && EF.Functions.ILike(x.NOMOR_POLISI, $"%{search}%")) ||
                        (x.NOMOR_BPKB != null && EF.Functions.ILike(x.NOMOR_BPKB, $"%{search}%")) ||
                        (x.NAMA_KWITANSI != null && EF.Functions.ILike(x.NAMA_KWITANSI, $"%{search}%"))
                    );

                    if (DateTime.TryParseExact(search, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        var ds = parsedDate.Date;
                        var de = ds.AddDays(1);
                        query = query.Where(x =>
                                (x.INSERT_DATE >= ds && x.INSERT_DATE < de) ||
                                (x.TANGGAL_AKTA.HasValue && x.TANGGAL_AKTA >= ds && x.TANGGAL_AKTA < de) ||
                                (x.TANGGAL_ORDER.HasValue && x.TANGGAL_ORDER >= ds && x.TANGGAL_ORDER < de) ||
                                (x.TANGGAL_KONTRAK.HasValue && x.TANGGAL_KONTRAK >= ds && x.TANGGAL_KONTRAK < de) ||
                                (x.TGL_SERTIFIKAT.HasValue && x.TGL_SERTIFIKAT >= ds && x.TGL_SERTIFIKAT < de)
                        );
                    }
                }
            }

            // ===============================
            // ORDER + LIMIT
            // ===============================

            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .OrderBy(x => x.ID)
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return new ResponseModel(ResponseCode.OK, "Success", total, dataList);
        }

        public async Task<ResponseModel> GetForMinuta_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {
            var startDate = Param.InsertDate.ToDateTime(TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);
            IQueryable<DataModels> query = from d in _dbContext.Documents.AsNoTracking()
                                           join p in _dbContext.Customers.AsNoTracking() on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                                           join c in _dbContext.Clients.AsNoTracking() on d.CLIENT_CODE equals c.CLIENT_CODE
                                           join t in _dbContext.Notaris.AsNoTracking() on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
                                           from t in ntrsDefault.DefaultIfEmpty()
                                           join b in _dbContext.Branches.AsNoTracking() on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
                                           from b in bcDefault.DefaultIfEmpty()
                                           where
                                             !d.DELETED_STATUS &&
                                             d.IMPORT_STATUS &&
                                             p.ISACTIVE &&
                                             c.ISACTIVE &&
                                             (
                                               (d.INSERT_DATE.HasValue && d.INSERT_DATE >= startDate && d.INSERT_DATE < endDate) ||
                                               (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE >= startDate && d.DUPLICATED_DATE < endDate)
                                             )
                                           select new DataModels
                                           {
                                               ID = d.ID,
                                               CLIENT_CODE = d.CLIENT_CODE,
                                               CLIENT_NAME = c.CLIENT_NAME,
                                               CUSTOMER_CODE = d.CUSTOMER_CODE,
                                               CUSTOMER_NAME = p.CUSTOMER_NAME,
                                               TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                                               BRANCH_CODE = d.BRANCH_CODE,
                                               BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                                               NOTARIS_CODE = d.NOTARIS_CODE,
                                               NOTARIS_NAME = t.NOTARIS_NAME,
                                               NO_REGISTRASI = d.NO_REGISTRASI,
                                               NO_VOUCHER = d.NO_VOUCHER,
                                               NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                                               NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                                               TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                                               JAM_MINUTA = d.JAM_MINUTA,
                                               TANGGAL_AKTA = d.TANGGAL_AKTA,
                                               NOMOR_AKTA = d.NOMOR_AKTA,
                                               TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                                               ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                                               NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                                               NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                                               JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                                               MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                                               TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                                               TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                                               PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                                               ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                                               RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                                               RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                                               KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                                               KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                                               KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                                               PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                                               POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                                               HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                                               ID_PASANGAN = d.ID_PASANGAN,
                                               NAMA_PASANGAN = d.NAMA_PASANGAN,
                                               JK_PASANGAN = d.JK_PASANGAN,
                                               MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                                               TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                                               TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                                               ID_DEBITUR = d.ID_DEBITUR,
                                               NAMA_DEBITUR = d.NAMA_DEBITUR,
                                               JK_DEBITUR = d.JK_DEBITUR,
                                               MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                                               TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                                               TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                                               ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                                               RT_DEBITUR = d.RT_DEBITUR,
                                               RW_DEBITUR = d.RW_DEBITUR,
                                               KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                                               KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                                               KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                                               PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                                               POS_DEBITUR = d.POS_DEBITUR,
                                               HP_DEBITUR = d.HP_DEBITUR,
                                               TANGGAL_ORDER = d.TANGGAL_ORDER,
                                               TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                                               NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                                               HUTANG_POKOK = d.HUTANG_POKOK,
                                               NILAI_JAMINAN = d.NILAI_JAMINAN,
                                               NILAI_BARANG = d.NILAI_BARANG,
                                               CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                                               JENIS_OBJECT = d.JENIS_OBJECT,
                                               MODEL = d.MODEL,
                                               MERK = d.MERK,
                                               TIPE = d.TIPE,
                                               TAHUN = d.TAHUN,
                                               WARNA = d.WARNA,
                                               NOMOR_RANGKA = d.NOMOR_RANGKA,
                                               NOMOR_MESIN = d.NOMOR_MESIN,
                                               NOMOR_POLISI = d.NOMOR_POLISI,
                                               NOMOR_BPKB = d.NOMOR_BPKB,
                                               NAMA_BPKB = d.NAMA_BPKB,
                                               PEMILIK_BPKB = d.PEMILIK_BPKB,
                                               TENOR = d.TENOR,
                                               TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                                               TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                                               TYPE_PRODUK = d.TYPE_PRODUK,
                                               WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                                               NAMA_KWITANSI = d.NAMA_KWITANSI,
                                               INSERT_DATE = d.INSERT_DATE,
                                               AHU_BY = d.AHU_BY,
                                               AHU_DATE = d.AHU_DATE,
                                               AHU_STATUS = d.AHU_STATUS,
                                               USER_BY = d.USER_BY,
                                               CERTIFICATE_BY = d.CERTIFICATE_BY,
                                               CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                                               CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                                               CREATED_BY = d.CREATED_BY,
                                               CREATED_DATE = d.CREATED_DATE,
                                               MODIFIED_BY = d.MODIFIED_BY,
                                               MODIFIED_DATE = d.MODIFIED_DATE,
                                               DELETED_STATUS = d.DELETED_STATUS,
                                               DELETED_DATE = d.DELETED_DATE,
                                               DELETED_BY = d.DELETED_BY,
                                               MESSAGES = d.MESSAGES,
                                               IS_DUPLICATE = d.IS_DUPLICATE,
                                               DUPLICATED_DATE = d.DUPLICATED_DATE,

                                               TIPE_CUSTOMER = p.TIPE,
                                               SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                                               TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                                               JENIS_CUSTOMER = p.JENIS,
                                               NPWP_CUSTOMER = p.NPWP,
                                               NIK_CUSTOMER = p.NIK,
                                               SK_CUSTOMER = p.SK,
                                               NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                                               TELP_CUSTOMER = p.TELP,
                                               EMAIL_CUSTOMER = p.EMAIL,
                                               KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                                               ALAMAT_CUSTOMER = p.ALAMAT,
                                               RT_CUSTOMER = p.RT,
                                               RW_CUSTOMER = p.RW,
                                               PROVINSI_CUSTOMER = p.PROVINSI,
                                               KOTA_CUSTOMER = p.KOTA,
                                               KECAMATAN_CUSTOMER = p.KECAMATAN,
                                               KELURAHAN_CUSTOMER = p.KELURAHAN,
                                               POS_CUSTOMER = p.POS,
                                               ISBRANCH = p.ISBRANCH,
                                               CODE_ALIAS = p.CODE_ALIAS
                                           };

            // ===============================
            // KEYSET CONDITION
            // ===============================
            //if (Param.StartRecord > 0)
            //{
            //    query = query.Where(x => x.ID > Param.StartRecord);
            //}

            // ===============================
            // User Filter
            // ===============================
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }


            // ===============================
            // Search (PostgreSQL Friendly)
            // ===============================
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                var search = Param.ParamSearch.Trim();
                if (bool.TryParse(search, out bool ahuStatus))
                {
                    query = query.Where(x => x.AHU_STATUS == ahuStatus);
                }
                else
                {
                    query = query.Where(x =>
                        (x.CLIENT_CODE != null && EF.Functions.ILike(x.CLIENT_CODE, $"%{search}%")) ||
                        (x.CLIENT_NAME != null && EF.Functions.ILike(x.CLIENT_NAME, $"%{search}%")) ||
                        (x.CUSTOMER_CODE != null && EF.Functions.ILike(x.CUSTOMER_CODE, $"%{search}%")) ||
                        (x.CUSTOMER_NAME != null && EF.Functions.ILike(x.CUSTOMER_NAME, $"%{search}%")) ||
                        (x.BRANCH_CODE != null && EF.Functions.ILike(x.BRANCH_CODE, $"%{search}%")) ||
                        (x.BRANCH_NAME != null && EF.Functions.ILike(x.BRANCH_NAME, $"%{search}%")) ||
                        (x.NOTARIS_CODE != null && EF.Functions.ILike(x.NOTARIS_CODE, $"%{search}%")) ||
                        (x.NOTARIS_NAME != null && EF.Functions.ILike(x.NOTARIS_NAME, $"%{search}%")) ||
                        //(x.NO_REGISTRASI != null && EF.Functions.ILike(x.NO_REGISTRASI, $"%{search}%")) ||
                        //(x.NO_VOUCHER != null && EF.Functions.ILike(x.NO_VOUCHER, $"%{search}%")) ||
                        //(x.NO_PEMBIAYAAN != null && EF.Functions.ILike(x.NO_PEMBIAYAAN, $"%{search}%")) ||
                        //(x.NO_SERTIFIKAT != null && EF.Functions.ILike(x.NO_SERTIFIKAT, $"%{search}%")) ||
                        (x.NOMOR_AKTA != null && EF.Functions.ILike(x.NOMOR_AKTA, $"%{search}%")) ||
                        (x.ID_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.ID_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NPWP_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NPWP_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NAMA_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PASANGAN != null && EF.Functions.ILike(x.NAMA_PASANGAN, $"%{search}%")) ||
                        (x.NAMA_DEBITUR != null && EF.Functions.ILike(x.NAMA_DEBITUR, $"%{search}%")) ||
                        (x.NOMOR_KONTRAK != null && EF.Functions.ILike(x.NOMOR_KONTRAK, $"%{search}%")) ||
                        (x.JENIS_OBJECT != null && EF.Functions.ILike(x.JENIS_OBJECT, $"%{search}%")) ||
                        (x.NOMOR_RANGKA != null && EF.Functions.ILike(x.NOMOR_RANGKA, $"%{search}%")) ||
                        (x.NOMOR_MESIN != null && EF.Functions.ILike(x.NOMOR_MESIN, $"%{search}%")) ||
                        (x.NOMOR_POLISI != null && EF.Functions.ILike(x.NOMOR_POLISI, $"%{search}%")) ||
                        (x.NOMOR_BPKB != null && EF.Functions.ILike(x.NOMOR_BPKB, $"%{search}%")) ||
                        (x.NAMA_KWITANSI != null && EF.Functions.ILike(x.NAMA_KWITANSI, $"%{search}%"))
                    );

                    if (DateTime.TryParseExact(search, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        var ds = parsedDate.Date;
                        var de = ds.AddDays(1);
                        query = query.Where(x =>
                                (x.INSERT_DATE >= ds && x.INSERT_DATE < de) ||
                                (x.TANGGAL_AKTA.HasValue && x.TANGGAL_AKTA >= ds && x.TANGGAL_AKTA < de) ||
                                (x.TANGGAL_ORDER.HasValue && x.TANGGAL_ORDER >= ds && x.TANGGAL_ORDER < de) ||
                                (x.TANGGAL_KONTRAK.HasValue && x.TANGGAL_KONTRAK >= ds && x.TANGGAL_KONTRAK < de) ||
                                (x.TGL_SERTIFIKAT.HasValue && x.TGL_SERTIFIKAT >= ds && x.TGL_SERTIFIKAT < de)
                        );
                    }
                }
            }

            // ===============================
            // ORDER + LIMIT
            // ===============================

            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .OrderBy(x => x.ID)
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return new ResponseModel(ResponseCode.OK, "Success", total, dataList);
        }


        public async Task<ResponseModel> GetForHistoryCertificates_Keyset(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {
            var startDate = Param.InsertDate.ToDateTime(TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);
            IQueryable<HISTORY_CERTIFICATE> query = from c in _dbContext.HistoriesCertificates.AsNoTracking()
                                                    where (c.CREATED_DATE.HasValue && c.CREATED_DATE >= startDate && c.CREATED_DATE < endDate) 
                                                    select c;

            // ===============================
            // KEYSET CONDITION
            // ===============================
            //if (Param.StartRecord > 0)
            //{
            //    query = query.Where(x => x.ID > Param.StartRecord);
            //}

            // ===============================
            // User Filter
            // ===============================
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }


            // ===============================
            // Search (PostgreSQL Friendly)
            // ===============================
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                var search = Param.ParamSearch.Trim();
                if (bool.TryParse(search, out bool sts))
                {
                    query = query.Where(x => (x.ISMATCH == sts) || x.STATUS_CERTIFICATE == sts);
                }
                else
                {
                    query = query.Where(x =>
                        (x.CLIENT_CODE != null && EF.Functions.ILike(x.CLIENT_CODE, $"%{search}%")) ||
                        (x.CUSTOMER_CODE != null && EF.Functions.ILike(x.CUSTOMER_CODE, $"%{search}%")) ||
                        (x.NO_REGISTRASI != null && EF.Functions.ILike(x.NO_REGISTRASI, $"%{search}%")) ||
                        (x.NO_VOUCHER != null && EF.Functions.ILike(x.NO_VOUCHER, $"%{search}%")) ||
                        (x.NO_PEMBIAYAAN != null && EF.Functions.ILike(x.NO_PEMBIAYAAN, $"%{search}%")) ||
                        (x.NO_SERTIFIKAT != null && EF.Functions.ILike(x.NO_SERTIFIKAT, $"%{search}%")) ||
                        (x.NOMOR_AKTA != null && EF.Functions.ILike(x.NOMOR_AKTA, $"%{search}%")) ||
                        (x.NOTARIS_CODE != null && EF.Functions.ILike(x.NOTARIS_CODE, $"%{search}%")) ||
                        (x.NOTARIS_NAME != null && EF.Functions.ILike(x.NOTARIS_NAME, $"%{search}%")) ||
                        (x.NPWP_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NPWP_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PEMBERIFIDUSIA != null && EF.Functions.ILike(x.NAMA_PEMBERIFIDUSIA, $"%{search}%")) ||
                        (x.NAMA_PENERIMAFIDUSIA != null && EF.Functions.ILike(x.NAMA_PENERIMAFIDUSIA, $"%{search}%")) ||
                        (x.NPWP_PENERIMAFIDUSIA != null && EF.Functions.ILike(x.NPWP_PENERIMAFIDUSIA, $"%{search}%")) ||
                        (x.CODE_PENERIMAFIDUSIA != null && EF.Functions.ILike(x.CODE_PENERIMAFIDUSIA, $"%{search}%")) ||
                        (x.WILAYAH != null && EF.Functions.ILike(x.WILAYAH, $"%{search}%")) ||
                        (x.BRANCH_CODE != null && EF.Functions.ILike(x.BRANCH_CODE, $"%{search}%")) ||
                        (x.BRANCH_NAME != null && EF.Functions.ILike(x.BRANCH_NAME, $"%{search}%")) ||
                        (x.FILENAME_CERTIFICATE != null && EF.Functions.ILike(x.FILENAME_CERTIFICATE, $"%{search}%")) 
                    );

                    if (DateTime.TryParseExact(search, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        var ds = parsedDate.Date;
                        var de = ds.AddDays(1);
                        query = query.Where(x =>
                                (x.TANGGAL_AKTA.HasValue && x.TANGGAL_AKTA >= ds && x.TANGGAL_AKTA < de) ||
                                (x.WAKTU_DAFTAR.HasValue && x.WAKTU_DAFTAR >= ds && x.WAKTU_DAFTAR < de) 
                        );
                    }
                }
            }

            // ===============================
            // ORDER + LIMIT
            // ===============================

            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .OrderBy(x => x.ID)
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return new ResponseModel(ResponseCode.OK, "Success", total, dataList);
        }



        public async Task<ResponseModel> GetForAHU(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {
            //var insertDateUtc = DateTime.SpecifyKind(Param.InsertDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var query = (
                from d in _dbContext.Documents
                join p in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE }  equals  new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                join c in _dbContext.Clients on d.CLIENT_CODE equals c.CLIENT_CODE
                join n in _dbContext.Notaris on d.CLIENT_CODE equals n.CLIENT_CODE
                join t in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
                from t in ntrsDefault.DefaultIfEmpty()
                join b in _dbContext.Branches on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
                from b in bcDefault.DefaultIfEmpty()
                where !d.DELETED_STATUS && d.IMPORT_STATUS
                    && d.INSERT_DATE.HasValue
                    //&& d.INSERT_DATE!.Value.Date == insertDateUtc.Date
                    && p.ISACTIVE && c.ISACTIVE && n.ISACTIVE
                    && (d.INSERT_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date ||
                       (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date))

                select new DataModels
                    {
                        ID = d.ID,
                        CLIENT_CODE = d.CLIENT_CODE,
                        CLIENT_NAME = c.CLIENT_NAME,
                        CUSTOMER_CODE = d.CUSTOMER_CODE,
                        CUSTOMER_NAME = p.CUSTOMER_NAME,
                        TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                        BRANCH_CODE = d.BRANCH_CODE,
                        BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                        NOTARIS_CODE = d.NOTARIS_CODE == null ? n.NOTARIS_CODE : d.NOTARIS_CODE, // CASE WHEN logic
                        NOTARIS_NAME = d.NOTARIS_CODE == null ? n.NOTARIS_NAME : t.NOTARIS_NAME, // CASE WHEN logic
                        NO_REGISTRASI = d.NO_REGISTRASI,
                        NO_VOUCHER = d.NO_VOUCHER,
                        NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                        NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                        TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                        JAM_MINUTA = d.JAM_MINUTA,
                        TANGGAL_AKTA = d.TANGGAL_AKTA,
                        NOMOR_AKTA = d.NOMOR_AKTA,
                        TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                        ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                        NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                        NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                        JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                        MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                        TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                        TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                        PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                        ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                        RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                        RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                        KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                        KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                        KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                        PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                        POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                        HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                        ID_PASANGAN = d.ID_PASANGAN,
                        NAMA_PASANGAN = d.NAMA_PASANGAN,
                        JK_PASANGAN = d.JK_PASANGAN,
                        MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                        TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                        TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                        ID_DEBITUR = d.ID_DEBITUR,
                        NAMA_DEBITUR = d.NAMA_DEBITUR,
                        JK_DEBITUR = d.JK_DEBITUR,
                        MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                        TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                        TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                        ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                        RT_DEBITUR = d.RT_DEBITUR,
                        RW_DEBITUR = d.RW_DEBITUR,
                        KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                        KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                        KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                        PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                        POS_DEBITUR = d.POS_DEBITUR,
                        HP_DEBITUR = d.HP_DEBITUR,
                        TANGGAL_ORDER = d.TANGGAL_ORDER,
                        TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                        NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                        HUTANG_POKOK = d.HUTANG_POKOK,
                        NILAI_JAMINAN = d.NILAI_JAMINAN,
                        NILAI_BARANG = d.NILAI_BARANG,
                        CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                        JENIS_OBJECT = d.JENIS_OBJECT,
                        MODEL = d.MODEL,
                        MERK = d.MERK,
                        TIPE = d.TIPE,
                        TAHUN = d.TAHUN,
                        WARNA = d.WARNA,
                        NOMOR_RANGKA = d.NOMOR_RANGKA,
                        NOMOR_MESIN = d.NOMOR_MESIN,
                        NOMOR_POLISI = d.NOMOR_POLISI,
                        NOMOR_BPKB = d.NOMOR_BPKB,
                        NAMA_BPKB = d.NAMA_BPKB,
                        PEMILIK_BPKB = d.PEMILIK_BPKB,
                        TENOR = d.TENOR,
                        TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                        TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                        TYPE_PRODUK = d.TYPE_PRODUK,
                        WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                        NAMA_KWITANSI = d.NAMA_KWITANSI,
                        INSERT_DATE = d.INSERT_DATE,
                        AHU_BY = d.AHU_BY,
                        AHU_DATE = d.AHU_DATE,
                        AHU_STATUS = d.AHU_STATUS,
                        USER_BY = d.USER_BY,
                        CERTIFICATE_BY = d.CERTIFICATE_BY,
                        CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                        CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                        CREATED_BY = d.CREATED_BY,
                        CREATED_DATE = d.CREATED_DATE,
                        MODIFIED_BY = d.MODIFIED_BY,
                        MODIFIED_DATE = d.MODIFIED_DATE,
                        DELETED_STATUS = d.DELETED_STATUS,
                        DELETED_DATE = d.DELETED_DATE,
                        DELETED_BY = d.DELETED_BY,
                        MESSAGES = d.MESSAGES,
                        IS_DUPLICATE = d.IS_DUPLICATE,
                        DUPLICATED_DATE = d.DUPLICATED_DATE,

                        TIPE_CUSTOMER = p.TIPE,
                        SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                        TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                        JENIS_CUSTOMER = p.JENIS,
                        NPWP_CUSTOMER = p.NPWP,
                        NIK_CUSTOMER = p.NIK,
                        SK_CUSTOMER = p.SK,
                        NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                        TELP_CUSTOMER = p.TELP,
                        EMAIL_CUSTOMER = p.EMAIL,
                        KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                        ALAMAT_CUSTOMER = p.ALAMAT,
                        RT_CUSTOMER = p.RT,
                        RW_CUSTOMER = p.RW,
                        PROVINSI_CUSTOMER = p.PROVINSI,
                        KOTA_CUSTOMER = p.KOTA,
                        KECAMATAN_CUSTOMER = p.KECAMATAN,
                        KELURAHAN_CUSTOMER = p.KELURAHAN,
                        POS_CUSTOMER = p.POS,
                        ISBRANCH = p.ISBRANCH,
                        CODE_ALIAS = p.CODE_ALIAS
                }).AsQueryable();

            // Filter by user type
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            // Filter by customer code
            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }

            // Search
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                string searchTerm = Param.ParamSearch;
                if (searchTerm.ToLower() == "true" || searchTerm.ToLower() == "false")
                {
                    bool ahuStatus = searchTerm == "true";
                    query = query.Where(d => d.AHU_STATUS == ahuStatus);
                }
                else
                {
                    query = query.Where(d =>
                        (d.CLIENT_CODE != null && d.CLIENT_CODE.Contains(searchTerm)) ||
                            (d.CLIENT_NAME != null && d.CLIENT_NAME.Contains(searchTerm)) ||
                            (d.CUSTOMER_CODE != null && d.CUSTOMER_CODE.Contains(searchTerm)) ||
                            (d.CUSTOMER_NAME != null && d.CUSTOMER_NAME.Contains(searchTerm)) ||
                            (d.BRANCH_CODE != null && d.BRANCH_CODE.Contains(searchTerm)) ||
                            (d.BRANCH_NAME != null && d.BRANCH_NAME.Contains(searchTerm)) ||
                            (d.NOTARIS_CODE != null && d.NOTARIS_CODE.Contains(searchTerm)) ||
                            (d.NOTARIS_NAME != null && d.NOTARIS_NAME.Contains(searchTerm)) ||
                            (d.NO_REGISTRASI != null && d.NO_REGISTRASI.Contains(searchTerm)) ||
                            (d.NO_VOUCHER != null && d.NO_VOUCHER.Contains(searchTerm)) ||
                            (d.NO_PEMBIAYAAN != null && d.NO_PEMBIAYAAN.Contains(searchTerm)) ||
                            (d.NO_SERTIFIKAT != null && d.NO_SERTIFIKAT.Contains(searchTerm)) ||
                           // (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_AKTA != null && d.NOMOR_AKTA.Contains(searchTerm)) ||
                            (d.ID_PEMBERIFIDUSIA != null && d.ID_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NPWP_PEMBERIFIDUSIA != null && d.NPWP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NAMA_PEMBERIFIDUSIA != null && d.NAMA_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.JK_PEMBERIFIDUSIA != null && d.JK_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.MARITAL_PEMBERIFIDUSIA != null && d.MARITAL_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.TPTLAHIR_PEMBERIFIDUSIA != null && d.TPTLAHIR_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.ToString().Contains(searchTerm)) ||
                           // (d.PEKERJAAN_PEMBERIFIDUSIA != null && d.PEKERJAAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.ALAMAT_PEMBERIFIDUSIA != null && d.ALAMAT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.RT_PEMBERIFIDUSIA != null && d.RT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.RW_PEMBERIFIDUSIA != null && d.RW_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.KELURAHAN_PEMBERIFIDUSIA != null && d.KELURAHAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.KECAMATAN_PEMBERIFIDUSIA != null && d.KECAMATAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.KABUPATEN_PEMBERIFIDUSIA != null && d.KABUPATEN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.PROVINSI_PEMBERIFIDUSIA != null && d.PROVINSI_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.POS_PEMBERIFIDUSIA != null && d.POS_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.HP_PEMBERIFIDUSIA != null && d.HP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                           // (d.ID_PASANGAN != null && d.ID_PASANGAN.Contains(searchTerm)) ||
                            (d.NAMA_PASANGAN != null && d.NAMA_PASANGAN.Contains(searchTerm)) ||
                           // (d.JK_PASANGAN != null && d.JK_PASANGAN.Contains(searchTerm)) ||
                           // (d.MARITAL_PASANGAN != null && d.MARITAL_PASANGAN.Contains(searchTerm)) ||
                           // (d.TPTLAHIR_PASANGAN != null && d.TPTLAHIR_PASANGAN.Contains(searchTerm)) ||
                           // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.ToString().Contains(searchTerm)) ||
                           // (d.ID_DEBITUR != null && d.ID_DEBITUR.Contains(searchTerm)) ||
                            (d.NAMA_DEBITUR != null && d.NAMA_DEBITUR.Contains(searchTerm)) ||
                           // (d.JK_DEBITUR != null && d.JK_DEBITUR.Contains(searchTerm)) ||
                           // (d.MARITAL_DEBITUR != null && d.MARITAL_DEBITUR.Contains(searchTerm)) ||
                           // (d.TPTLAHIR_DEBITUR != null && d.TPTLAHIR_DEBITUR.Contains(searchTerm)) ||
                           // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.ToString().Contains(searchTerm)) ||
                           // (d.ALAMAT_DEBITUR != null && d.ALAMAT_DEBITUR.Contains(searchTerm)) ||
                           // (d.RT_DEBITUR != null && d.RT_DEBITUR.Contains(searchTerm)) ||
                           // (d.RW_DEBITUR != null && d.RW_DEBITUR.Contains(searchTerm)) ||
                           // (d.KELURAHAN_DEBITUR != null && d.KELURAHAN_DEBITUR.Contains(searchTerm)) ||
                           // (d.KECAMATAN_DEBITUR != null && d.KECAMATAN_DEBITUR.Contains(searchTerm)) ||
                           // (d.KABUPATEN_DEBITUR != null && d.KABUPATEN_DEBITUR.Contains(searchTerm)) ||
                           // (d.PROVINSI_DEBITUR != null && d.PROVINSI_DEBITUR.Contains(searchTerm)) ||
                           // (d.POS_DEBITUR != null && d.POS_DEBITUR.Contains(searchTerm)) ||
                           // (d.HP_DEBITUR != null && d.HP_DEBITUR.Contains(searchTerm)) ||
                           // (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.ToString().Contains(searchTerm)) ||
                           // (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_KONTRAK != null && d.NOMOR_KONTRAK.Contains(searchTerm)) ||
                           // (d.HUTANG_POKOK.HasValue && d.HUTANG_POKOK.Value.ToString().Contains(searchTerm)) ||
                           // (d.NILAI_JAMINAN.HasValue && d.NILAI_JAMINAN.Value.ToString().Contains(searchTerm)) ||
                           // (d.NILAI_BARANG.HasValue && d.NILAI_BARANG.Value.ToString().Contains(searchTerm)) ||
                            (d.JENIS_OBJECT != null && d.JENIS_OBJECT.Contains(searchTerm)) ||
                           // (d.MODEL != null && d.MODEL.Contains(searchTerm)) ||
                           // (d.MERK != null && d.MERK.Contains(searchTerm)) ||
                           // (d.TIPE != null && d.TIPE.Contains(searchTerm)) ||
                           // (d.TAHUN.HasValue && d.TAHUN.Value.ToString().Contains(searchTerm)) ||
                           // (d.WARNA != null && d.WARNA.Contains(searchTerm)) ||
                            (d.NOMOR_RANGKA != null && d.NOMOR_RANGKA.Contains(searchTerm)) ||
                            (d.NOMOR_MESIN != null && d.NOMOR_MESIN.Contains(searchTerm)) ||
                            (d.NOMOR_POLISI != null && d.NOMOR_POLISI.Contains(searchTerm)) ||
                            (d.NOMOR_BPKB != null && d.NOMOR_BPKB.Contains(searchTerm)) ||
                           // (d.NAMA_BPKB != null && d.NAMA_BPKB.Contains(searchTerm)) ||
                           // (d.PEMILIK_BPKB != null && d.PEMILIK_BPKB.Contains(searchTerm)) ||
                           // (d.TENOR != null && d.TENOR.Contains(searchTerm)) ||
                           // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.ToString().Contains(searchTerm)) ||
                           // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.ToString().Contains(searchTerm)) ||
                           // (d.TYPE_PRODUK != null && d.TYPE_PRODUK.Contains(searchTerm)) ||
                           // (d.WAY_OF_FINANCING != null && d.WAY_OF_FINANCING.Contains(searchTerm)) ||
                           // (d.USER_BY != null && d.USER_BY.Contains(searchTerm)) ||
                           // (d.MESSAGES != null && d.MESSAGES.Contains(searchTerm)) ||
                            (d.NAMA_KWITANSI != null && d.NAMA_KWITANSI.Contains(searchTerm))
                    );

                    if (DateTime.TryParseExact(searchTerm, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        query = query.Where(d =>
                                (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.Date == parsedDate.Date||
                                (d.TGL_SERTIFIKAT.HasValue && d.TGL_SERTIFIKAT.Value.Date == parsedDate.Date)
                        );
                    }
                }
            }

            // Sorting
            if (!String.IsNullOrEmpty(Param.SortBy) && !String.IsNullOrEmpty(Param.SortValue))
            {
                var Sort = Param.SortBy;
                var sortProperty = typeof(DataModels).GetProperty(Param.SortBy);
                if (sortProperty != null)
                {
                    if (Param.SortValue == "ASC")
                    {
                        query = query.OrderBy(d => sortProperty.GetValue(d));
                    }
                    else
                    {
                        query = query.OrderByDescending(d => sortProperty.GetValue(d));
                    }
                }
            }
            else
            {
                //query = query.OrderByDescending(d => d.INSERT_DATE).ThenBy(d => d.ID);
                query = query.OrderBy(d => d.ID);
            }


            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync();
            var result = new ResponseModel(ResponseCode.OK, "Success", total, dataList);
            return result;
            
        }
        
        public async Task<ResponseModel> GetForCertificate(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {
            var query = (
               from d in _dbContext.Documents
               join p in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
               join c in _dbContext.Clients on d.CLIENT_CODE equals c.CLIENT_CODE
               join n in _dbContext.Notaris on d.CLIENT_CODE equals n.CLIENT_CODE
               join t in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
               from t in ntrsDefault.DefaultIfEmpty()
               join b in _dbContext.Branches on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
               from b in bcDefault.DefaultIfEmpty()
               where !d.DELETED_STATUS 
                   && d.AHU_STATUS
                   && d.IMPORT_STATUS
                   && d.INSERT_DATE.HasValue
                   && p.ISACTIVE && c.ISACTIVE && n.ISACTIVE
                   && (d.INSERT_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date ||
                       (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date))
               select new DataModels
                {
                    ID = d.ID,
                    CLIENT_CODE = d.CLIENT_CODE,
                    CLIENT_NAME = c.CLIENT_NAME,
                    CUSTOMER_CODE = d.CUSTOMER_CODE,
                    CUSTOMER_NAME = p.CUSTOMER_NAME,
                    TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                    BRANCH_CODE = d.BRANCH_CODE,
                    BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                    NOTARIS_CODE = d.NOTARIS_CODE == null ? n.NOTARIS_CODE : d.NOTARIS_CODE, // CASE WHEN logic
                    NOTARIS_NAME = d.NOTARIS_CODE == null ? n.NOTARIS_NAME : t.NOTARIS_NAME, // CASE WHEN logic
                    NO_REGISTRASI = d.NO_REGISTRASI,
                    NO_VOUCHER = d.NO_VOUCHER,
                    NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                    NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                    TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                    JAM_MINUTA = d.JAM_MINUTA,
                    TANGGAL_AKTA = d.TANGGAL_AKTA,
                    NOMOR_AKTA = d.NOMOR_AKTA,
                    TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                    ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                    NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                    NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                    JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                    MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                    TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                    TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                    PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                    ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                    RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                    RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                    KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                    KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                    KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                    PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                    POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                    HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                    ID_PASANGAN = d.ID_PASANGAN,
                    NAMA_PASANGAN = d.NAMA_PASANGAN,
                    JK_PASANGAN = d.JK_PASANGAN,
                    MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                    TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                    TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                    ID_DEBITUR = d.ID_DEBITUR,
                    NAMA_DEBITUR = d.NAMA_DEBITUR,
                    JK_DEBITUR = d.JK_DEBITUR,
                    MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                    TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                    TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                    ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                    RT_DEBITUR = d.RT_DEBITUR,
                    RW_DEBITUR = d.RW_DEBITUR,
                    KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                    KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                    KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                    PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                    POS_DEBITUR = d.POS_DEBITUR,
                    HP_DEBITUR = d.HP_DEBITUR,
                    TANGGAL_ORDER = d.TANGGAL_ORDER,
                    TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                    NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                    HUTANG_POKOK = d.HUTANG_POKOK,
                    NILAI_JAMINAN = d.NILAI_JAMINAN,
                    NILAI_BARANG = d.NILAI_BARANG,
                    CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                    JENIS_OBJECT = d.JENIS_OBJECT,
                    MODEL = d.MODEL,
                    MERK = d.MERK,
                    TIPE = d.TIPE,
                    TAHUN = d.TAHUN,
                    WARNA = d.WARNA,
                    NOMOR_RANGKA = d.NOMOR_RANGKA,
                    NOMOR_MESIN = d.NOMOR_MESIN,
                    NOMOR_POLISI = d.NOMOR_POLISI,
                    NOMOR_BPKB = d.NOMOR_BPKB,
                    NAMA_BPKB = d.NAMA_BPKB,
                    PEMILIK_BPKB = d.PEMILIK_BPKB,
                    TENOR = d.TENOR,
                    TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                    TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                    TYPE_PRODUK = d.TYPE_PRODUK,
                    WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                    NAMA_KWITANSI = d.NAMA_KWITANSI,
                    INSERT_DATE = d.INSERT_DATE,
                    AHU_BY = d.AHU_BY,
                    AHU_DATE = d.AHU_DATE,
                    AHU_STATUS = d.AHU_STATUS,
                    USER_BY = d.USER_BY,
                    CERTIFICATE_BY = d.CERTIFICATE_BY,
                    CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                    CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                    CREATED_BY = d.CREATED_BY,
                    CREATED_DATE = d.CREATED_DATE,
                    MODIFIED_BY = d.MODIFIED_BY,
                    MODIFIED_DATE = d.MODIFIED_DATE,
                    DELETED_STATUS = d.DELETED_STATUS,
                    DELETED_DATE = d.DELETED_DATE,
                    DELETED_BY = d.DELETED_BY,
                    MESSAGES = d.MESSAGES,
                    IS_DUPLICATE = d.IS_DUPLICATE,
                    DUPLICATED_DATE = d.DUPLICATED_DATE,

                    TIPE_CUSTOMER = p.TIPE,
                    SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                    TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                    JENIS_CUSTOMER = p.JENIS,
                    NPWP_CUSTOMER = p.NPWP,
                    NIK_CUSTOMER = p.NIK,
                    SK_CUSTOMER = p.SK,
                    NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                    TELP_CUSTOMER = p.TELP,
                    EMAIL_CUSTOMER = p.EMAIL,
                    KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                    ALAMAT_CUSTOMER = p.ALAMAT,
                    RT_CUSTOMER = p.RT,
                    RW_CUSTOMER = p.RW,
                    PROVINSI_CUSTOMER = p.PROVINSI,
                    KOTA_CUSTOMER = p.KOTA,
                    KECAMATAN_CUSTOMER = p.KECAMATAN,
                    KELURAHAN_CUSTOMER = p.KELURAHAN,
                    POS_CUSTOMER = p.POS,
                   ISBRANCH = p.ISBRANCH,
                   CODE_ALIAS = p.CODE_ALIAS

               }).AsQueryable();
            // Filter by user type
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            // Filter by customer code
            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }

            // Search
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                string searchTerm = Param.ParamSearch;
                if (searchTerm.ToLower() == "true" || searchTerm.ToLower() == "false")
                {
                    bool cerStatus = searchTerm == "true";
                    query = query.Where(d => d.CERTIFICATE_STATUS == cerStatus);
                }
                else
                {
                    query = query.Where(d =>
                        (d.CLIENT_CODE != null && d.CLIENT_CODE.Contains(searchTerm)) ||
                            (d.CLIENT_NAME != null && d.CLIENT_NAME.Contains(searchTerm)) ||
                            (d.CUSTOMER_CODE != null && d.CUSTOMER_CODE.Contains(searchTerm)) ||
                            (d.CUSTOMER_NAME != null && d.CUSTOMER_NAME.Contains(searchTerm)) ||
                            (d.BRANCH_CODE != null && d.BRANCH_CODE.Contains(searchTerm)) ||
                            (d.BRANCH_NAME != null && d.BRANCH_NAME.Contains(searchTerm)) ||
                            (d.NOTARIS_CODE != null && d.NOTARIS_CODE.Contains(searchTerm)) ||
                            (d.NOTARIS_NAME != null && d.NOTARIS_NAME.Contains(searchTerm)) ||
                            (d.NO_REGISTRASI != null && d.NO_REGISTRASI.Contains(searchTerm)) ||
                            (d.NO_VOUCHER != null && d.NO_VOUCHER.Contains(searchTerm)) ||
                            (d.NO_PEMBIAYAAN != null && d.NO_PEMBIAYAAN.Contains(searchTerm)) ||
                            (d.NO_SERTIFIKAT != null && d.NO_SERTIFIKAT.Contains(searchTerm)) ||
                            // (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_AKTA != null && d.NOMOR_AKTA.Contains(searchTerm)) ||
                            (d.ID_PEMBERIFIDUSIA != null && d.ID_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NPWP_PEMBERIFIDUSIA != null && d.NPWP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NAMA_PEMBERIFIDUSIA != null && d.NAMA_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.JK_PEMBERIFIDUSIA != null && d.JK_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.MARITAL_PEMBERIFIDUSIA != null && d.MARITAL_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_PEMBERIFIDUSIA != null && d.TPTLAHIR_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.ToString().Contains(searchTerm)) ||
                            // (d.PEKERJAAN_PEMBERIFIDUSIA != null && d.PEKERJAAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.ALAMAT_PEMBERIFIDUSIA != null && d.ALAMAT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.RT_PEMBERIFIDUSIA != null && d.RT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.RW_PEMBERIFIDUSIA != null && d.RW_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KELURAHAN_PEMBERIFIDUSIA != null && d.KELURAHAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KECAMATAN_PEMBERIFIDUSIA != null && d.KECAMATAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KABUPATEN_PEMBERIFIDUSIA != null && d.KABUPATEN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.PROVINSI_PEMBERIFIDUSIA != null && d.PROVINSI_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.POS_PEMBERIFIDUSIA != null && d.POS_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.HP_PEMBERIFIDUSIA != null && d.HP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.ID_PASANGAN != null && d.ID_PASANGAN.Contains(searchTerm)) ||
                            (d.NAMA_PASANGAN != null && d.NAMA_PASANGAN.Contains(searchTerm)) ||
                            // (d.JK_PASANGAN != null && d.JK_PASANGAN.Contains(searchTerm)) ||
                            // (d.MARITAL_PASANGAN != null && d.MARITAL_PASANGAN.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_PASANGAN != null && d.TPTLAHIR_PASANGAN.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.ToString().Contains(searchTerm)) ||
                            // (d.ID_DEBITUR != null && d.ID_DEBITUR.Contains(searchTerm)) ||
                            (d.NAMA_DEBITUR != null && d.NAMA_DEBITUR.Contains(searchTerm)) ||
                            // (d.JK_DEBITUR != null && d.JK_DEBITUR.Contains(searchTerm)) ||
                            // (d.MARITAL_DEBITUR != null && d.MARITAL_DEBITUR.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_DEBITUR != null && d.TPTLAHIR_DEBITUR.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.ToString().Contains(searchTerm)) ||
                            // (d.ALAMAT_DEBITUR != null && d.ALAMAT_DEBITUR.Contains(searchTerm)) ||
                            // (d.RT_DEBITUR != null && d.RT_DEBITUR.Contains(searchTerm)) ||
                            // (d.RW_DEBITUR != null && d.RW_DEBITUR.Contains(searchTerm)) ||
                            // (d.KELURAHAN_DEBITUR != null && d.KELURAHAN_DEBITUR.Contains(searchTerm)) ||
                            // (d.KECAMATAN_DEBITUR != null && d.KECAMATAN_DEBITUR.Contains(searchTerm)) ||
                            // (d.KABUPATEN_DEBITUR != null && d.KABUPATEN_DEBITUR.Contains(searchTerm)) ||
                            // (d.PROVINSI_DEBITUR != null && d.PROVINSI_DEBITUR.Contains(searchTerm)) ||
                            // (d.POS_DEBITUR != null && d.POS_DEBITUR.Contains(searchTerm)) ||
                            // (d.HP_DEBITUR != null && d.HP_DEBITUR.Contains(searchTerm)) ||
                            // (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.ToString().Contains(searchTerm)) ||
                            // (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_KONTRAK != null && d.NOMOR_KONTRAK.Contains(searchTerm)) ||
                            // (d.HUTANG_POKOK.HasValue && d.HUTANG_POKOK.Value.ToString().Contains(searchTerm)) ||
                            // (d.NILAI_JAMINAN.HasValue && d.NILAI_JAMINAN.Value.ToString().Contains(searchTerm)) ||
                            // (d.NILAI_BARANG.HasValue && d.NILAI_BARANG.Value.ToString().Contains(searchTerm)) ||
                            (d.JENIS_OBJECT != null && d.JENIS_OBJECT.Contains(searchTerm)) ||
                            // (d.MODEL != null && d.MODEL.Contains(searchTerm)) ||
                            // (d.MERK != null && d.MERK.Contains(searchTerm)) ||
                            // (d.TIPE != null && d.TIPE.Contains(searchTerm)) ||
                            // (d.TAHUN.HasValue && d.TAHUN.Value.ToString().Contains(searchTerm)) ||
                            // (d.WARNA != null && d.WARNA.Contains(searchTerm)) ||
                            (d.NOMOR_RANGKA != null && d.NOMOR_RANGKA.Contains(searchTerm)) ||
                            (d.NOMOR_MESIN != null && d.NOMOR_MESIN.Contains(searchTerm)) ||
                            (d.NOMOR_POLISI != null && d.NOMOR_POLISI.Contains(searchTerm)) ||
                            (d.NOMOR_BPKB != null && d.NOMOR_BPKB.Contains(searchTerm)) ||
                            // (d.NAMA_BPKB != null && d.NAMA_BPKB.Contains(searchTerm)) ||
                            // (d.PEMILIK_BPKB != null && d.PEMILIK_BPKB.Contains(searchTerm)) ||
                            // (d.TENOR != null && d.TENOR.Contains(searchTerm)) ||
                            // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.ToString().Contains(searchTerm)) ||
                            // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.ToString().Contains(searchTerm)) ||
                            (d.TYPE_PRODUK != null && d.TYPE_PRODUK.Contains(searchTerm)) ||
                            // (d.WAY_OF_FINANCING != null && d.WAY_OF_FINANCING.Contains(searchTerm)) ||
                            // (d.USER_BY != null && d.USER_BY.Contains(searchTerm)) ||
                            // (d.MESSAGES != null && d.MESSAGES.Contains(searchTerm)) ||
                            (d.NAMA_KWITANSI != null && d.NAMA_KWITANSI.Contains(searchTerm))
                    );

                    if (DateTime.TryParseExact(searchTerm, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        query = query.Where(d =>
                                (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.Date == parsedDate.Date||
                                (d.TGL_SERTIFIKAT.HasValue && d.TGL_SERTIFIKAT.Value.Date == parsedDate.Date)
                        );
                    }
                }
            }

            // Sorting
            if (!String.IsNullOrEmpty(Param.SortBy) && !String.IsNullOrEmpty(Param.SortValue))
            {
                var Sort = Param.SortBy;
                var sortProperty = typeof(DataModels).GetProperty(Param.SortBy);
                if (sortProperty != null)
                {
                    if (Param.SortValue == "ASC")
                    {
                        query = query.OrderBy(d => sortProperty.GetValue(d));
                    }
                    else
                    {
                        query = query.OrderByDescending(d => sortProperty.GetValue(d));
                    }
                }
            }
            else
            {
                //query = query.OrderByDescending(d => d.INSERT_DATE).ThenBy(d => d.ID);
                query = query.OrderBy(d => d.ID);
            }


            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync();
            var result = new ResponseModel(ResponseCode.OK, "Success", total, dataList);
            return result;
        }

        public async Task<ResponseModel> GetForMinuta(Principal UserCurrent, ParamData Param, CancellationToken cancellationToken = default)
        {
            var query = (
                from d in _dbContext.Documents
                join p in _dbContext.Customers on new { d.CUSTOMER_CODE, d.CLIENT_CODE } equals new { p.CUSTOMER_CODE, p.CLIENT_CODE }
                join c in _dbContext.Clients on d.CLIENT_CODE equals c.CLIENT_CODE
                join n in _dbContext.Notaris on d.CLIENT_CODE equals n.CLIENT_CODE
                join t in _dbContext.Notaris on new { d.NOTARIS_CODE, d.CLIENT_CODE } equals new { t.NOTARIS_CODE, t.CLIENT_CODE } into ntrsDefault
                from t in ntrsDefault.DefaultIfEmpty()
                join b in _dbContext.Branches on d.BRANCH_CODE equals b.BRANCH_CODE into bcDefault
                from b in bcDefault.DefaultIfEmpty()
                where !d.DELETED_STATUS
                    && d.IMPORT_STATUS
                    && d.INSERT_DATE.HasValue
                    && p.ISACTIVE && c.ISACTIVE && n.ISACTIVE
                    && (d.INSERT_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date ||
                       (d.IS_DUPLICATE && d.DUPLICATED_DATE.HasValue && d.DUPLICATED_DATE!.Value.Date == Param.InsertDate.ToDateTime(TimeOnly.MinValue).Date))

                select new DataModels
                {
                    ID = d.ID,
                    CLIENT_CODE = d.CLIENT_CODE,
                    CLIENT_NAME = c.CLIENT_NAME,
                    CUSTOMER_CODE = d.CUSTOMER_CODE,
                    CUSTOMER_NAME = p.CUSTOMER_NAME,
                    TYPE_FIDUSIA = d.TYPE_FIDUSIA,
                    BRANCH_CODE = d.BRANCH_CODE,
                    BRANCH_NAME = b.BRANCH_NAME ?? d.BRANCH_CODE,
                    NOTARIS_CODE = d.NOTARIS_CODE == null ? n.NOTARIS_CODE : d.NOTARIS_CODE, // CASE WHEN logic
                    NOTARIS_NAME = d.NOTARIS_CODE == null ? n.NOTARIS_NAME : t.NOTARIS_NAME, // CASE WHEN logic
                    NO_REGISTRASI = d.NO_REGISTRASI,
                    NO_VOUCHER = d.NO_VOUCHER,
                    NO_PEMBIAYAAN = d.NO_PEMBIAYAAN,
                    NO_SERTIFIKAT = d.NO_SERTIFIKAT,
                    TGL_SERTIFIKAT = d.TGL_SERTIFIKAT,
                    JAM_MINUTA = d.JAM_MINUTA,
                    TANGGAL_AKTA = d.TANGGAL_AKTA,
                    NOMOR_AKTA = d.NOMOR_AKTA,
                    TIPE_PEMBERIFIDUSIA = d.TIPE_PEMBERIFIDUSIA,
                    ID_PEMBERIFIDUSIA = d.ID_PEMBERIFIDUSIA,
                    NPWP_PEMBERIFIDUSIA = d.NPWP_PEMBERIFIDUSIA,
                    NAMA_PEMBERIFIDUSIA = d.NAMA_PEMBERIFIDUSIA,
                    JK_PEMBERIFIDUSIA = d.JK_PEMBERIFIDUSIA,
                    MARITAL_PEMBERIFIDUSIA = d.MARITAL_PEMBERIFIDUSIA,
                    TPTLAHIR_PEMBERIFIDUSIA = d.TPTLAHIR_PEMBERIFIDUSIA,
                    TGLLAHIR_PEMBERIFIDUSIA = d.TGLLAHIR_PEMBERIFIDUSIA,
                    PEKERJAAN_PEMBERIFIDUSIA = d.PEKERJAAN_PEMBERIFIDUSIA,
                    ALAMAT_PEMBERIFIDUSIA = d.ALAMAT_PEMBERIFIDUSIA,
                    RT_PEMBERIFIDUSIA = d.RT_PEMBERIFIDUSIA,
                    RW_PEMBERIFIDUSIA = d.RW_PEMBERIFIDUSIA,
                    KELURAHAN_PEMBERIFIDUSIA = d.KELURAHAN_PEMBERIFIDUSIA,
                    KECAMATAN_PEMBERIFIDUSIA = d.KECAMATAN_PEMBERIFIDUSIA,
                    KABUPATEN_PEMBERIFIDUSIA = d.KABUPATEN_PEMBERIFIDUSIA,
                    PROVINSI_PEMBERIFIDUSIA = d.PROVINSI_PEMBERIFIDUSIA,
                    POS_PEMBERIFIDUSIA = d.POS_PEMBERIFIDUSIA,
                    HP_PEMBERIFIDUSIA = d.HP_PEMBERIFIDUSIA,
                    ID_PASANGAN = d.ID_PASANGAN,
                    NAMA_PASANGAN = d.NAMA_PASANGAN,
                    JK_PASANGAN = d.JK_PASANGAN,
                    MARITAL_PASANGAN = d.MARITAL_PASANGAN,
                    TPTLAHIR_PASANGAN = d.TPTLAHIR_PASANGAN,
                    TGLLAHIR_PASANGAN = d.TGLLAHIR_PASANGAN,
                    ID_DEBITUR = d.ID_DEBITUR,
                    NAMA_DEBITUR = d.NAMA_DEBITUR,
                    JK_DEBITUR = d.JK_DEBITUR,
                    MARITAL_DEBITUR = d.MARITAL_DEBITUR,
                    TPTLAHIR_DEBITUR = d.TPTLAHIR_DEBITUR,
                    TGLLAHIR_DEBITUR = d.TGLLAHIR_DEBITUR,
                    ALAMAT_DEBITUR = d.ALAMAT_DEBITUR,
                    RT_DEBITUR = d.RT_DEBITUR,
                    RW_DEBITUR = d.RW_DEBITUR,
                    KELURAHAN_DEBITUR = d.KELURAHAN_DEBITUR,
                    KECAMATAN_DEBITUR = d.KECAMATAN_DEBITUR,
                    KABUPATEN_DEBITUR = d.KABUPATEN_DEBITUR,
                    PROVINSI_DEBITUR = d.PROVINSI_DEBITUR,
                    POS_DEBITUR = d.POS_DEBITUR,
                    HP_DEBITUR = d.HP_DEBITUR,
                    TANGGAL_ORDER = d.TANGGAL_ORDER,
                    TANGGAL_KONTRAK = d.TANGGAL_KONTRAK,
                    NOMOR_KONTRAK = d.NOMOR_KONTRAK,
                    HUTANG_POKOK = d.HUTANG_POKOK,
                    NILAI_JAMINAN = d.NILAI_JAMINAN,
                    NILAI_BARANG = d.NILAI_BARANG,
                    CATEGORY_OBJECT = d.CATEGORY_OBJECT,
                    JENIS_OBJECT = d.JENIS_OBJECT,
                    MODEL = d.MODEL,
                    MERK = d.MERK,
                    TIPE = d.TIPE,
                    TAHUN = d.TAHUN,
                    WARNA = d.WARNA,
                    NOMOR_RANGKA = d.NOMOR_RANGKA,
                    NOMOR_MESIN = d.NOMOR_MESIN,
                    NOMOR_POLISI = d.NOMOR_POLISI,
                    NOMOR_BPKB = d.NOMOR_BPKB,
                    NAMA_BPKB = d.NAMA_BPKB,
                    PEMILIK_BPKB = d.PEMILIK_BPKB,
                    TENOR = d.TENOR,
                    TANGGAL_AWAL_TENOR = d.TANGGAL_AWAL_TENOR,
                    TANGGAL_AKHIR_TENOR = d.TANGGAL_AKHIR_TENOR,
                    TYPE_PRODUK = d.TYPE_PRODUK,
                    WAY_OF_FINANCING = d.WAY_OF_FINANCING,
                    NAMA_KWITANSI = d.NAMA_KWITANSI,
                    INSERT_DATE = d.INSERT_DATE,
                    AHU_BY = d.AHU_BY,
                    AHU_DATE = d.AHU_DATE,
                    AHU_STATUS = d.AHU_STATUS,
                    USER_BY = d.USER_BY,
                    CERTIFICATE_BY = d.CERTIFICATE_BY,
                    CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                    CERTIFICATE_STATUS = d.CERTIFICATE_STATUS,
                    CREATED_BY = d.CREATED_BY,
                    CREATED_DATE = d.CREATED_DATE,
                    MODIFIED_BY = d.MODIFIED_BY,
                    MODIFIED_DATE = d.MODIFIED_DATE,
                    DELETED_STATUS = d.DELETED_STATUS,
                    DELETED_DATE = d.DELETED_DATE,
                    DELETED_BY = d.DELETED_BY,
                    MESSAGES = d.MESSAGES,
                    IS_DUPLICATE = d.IS_DUPLICATE,
                    DUPLICATED_DATE = d.DUPLICATED_DATE,

                    TIPE_CUSTOMER = p.TIPE,
                    SUB_TIPE_CUSTOMER = p.SUB_TIPE,
                    TIPE_NAME_CUSTOMER = p.TIPE_NAME,
                    JENIS_CUSTOMER = p.JENIS,
                    NPWP_CUSTOMER = p.NPWP,
                    NIK_CUSTOMER = p.NIK,
                    SK_CUSTOMER = p.SK,
                    NEGARA_ASAL_CUSTOMER = p.NEGARA_ASAL,
                    TELP_CUSTOMER = p.TELP,
                    EMAIL_CUSTOMER = p.EMAIL,
                    KANTOR_CABANG_CUSTOMER = p.KANTOR_CABANG,
                    ALAMAT_CUSTOMER = p.ALAMAT,
                    RT_CUSTOMER = p.RT,
                    RW_CUSTOMER = p.RW,
                    PROVINSI_CUSTOMER = p.PROVINSI,
                    KOTA_CUSTOMER = p.KOTA,
                    KECAMATAN_CUSTOMER = p.KECAMATAN,
                    KELURAHAN_CUSTOMER = p.KELURAHAN,
                    POS_CUSTOMER = p.POS,
                    ISBRANCH = p.ISBRANCH,
                    CODE_ALIAS = p.CODE_ALIAS
                }).AsQueryable();

            // Filter by user type
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }

            // Filter by customer code
            if (!string.IsNullOrEmpty(Param.CustomerCode))
            {
                query = query.Where(x => x.CUSTOMER_CODE == Param.CustomerCode);
            }

            // Search
            if (!string.IsNullOrEmpty(Param.ParamSearch))
            {
                string searchTerm = Param.ParamSearch;
                if (searchTerm.ToLower() == "true" || searchTerm.ToLower() == "false")
                {
                    bool ahuStatus = searchTerm == "true";
                    query = query.Where(d => d.AHU_STATUS == ahuStatus || d.CERTIFICATE_STATUS == ahuStatus);
                }
                else
                {
                    query = query.Where(d =>
                        (d.CLIENT_CODE != null && d.CLIENT_CODE.Contains(searchTerm)) ||
                            (d.CLIENT_NAME != null && d.CLIENT_NAME.Contains(searchTerm)) ||
                            (d.CUSTOMER_CODE != null && d.CUSTOMER_CODE.Contains(searchTerm)) ||
                            (d.CUSTOMER_NAME != null && d.CUSTOMER_NAME.Contains(searchTerm)) ||
                            (d.BRANCH_CODE != null && d.BRANCH_CODE.Contains(searchTerm)) ||
                            (d.BRANCH_NAME != null && d.BRANCH_NAME.Contains(searchTerm)) ||
                            (d.NOTARIS_CODE != null && d.NOTARIS_CODE.Contains(searchTerm)) ||
                            (d.NOTARIS_NAME != null && d.NOTARIS_NAME.Contains(searchTerm)) ||
                            (d.NO_REGISTRASI != null && d.NO_REGISTRASI.Contains(searchTerm)) ||
                            (d.NO_VOUCHER != null && d.NO_VOUCHER.Contains(searchTerm)) ||
                            (d.NO_PEMBIAYAAN != null && d.NO_PEMBIAYAAN.Contains(searchTerm)) ||
                            (d.NO_SERTIFIKAT != null && d.NO_SERTIFIKAT.Contains(searchTerm)) ||
                            // (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_AKTA != null && d.NOMOR_AKTA.Contains(searchTerm)) ||
                            (d.ID_PEMBERIFIDUSIA != null && d.ID_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NPWP_PEMBERIFIDUSIA != null && d.NPWP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            (d.NAMA_PEMBERIFIDUSIA != null && d.NAMA_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.JK_PEMBERIFIDUSIA != null && d.JK_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.MARITAL_PEMBERIFIDUSIA != null && d.MARITAL_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_PEMBERIFIDUSIA != null && d.TPTLAHIR_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.ToString().Contains(searchTerm)) ||
                            // (d.PEKERJAAN_PEMBERIFIDUSIA != null && d.PEKERJAAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.ALAMAT_PEMBERIFIDUSIA != null && d.ALAMAT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.RT_PEMBERIFIDUSIA != null && d.RT_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.RW_PEMBERIFIDUSIA != null && d.RW_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KELURAHAN_PEMBERIFIDUSIA != null && d.KELURAHAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KECAMATAN_PEMBERIFIDUSIA != null && d.KECAMATAN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.KABUPATEN_PEMBERIFIDUSIA != null && d.KABUPATEN_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.PROVINSI_PEMBERIFIDUSIA != null && d.PROVINSI_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.POS_PEMBERIFIDUSIA != null && d.POS_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.HP_PEMBERIFIDUSIA != null && d.HP_PEMBERIFIDUSIA.Contains(searchTerm)) ||
                            // (d.ID_PASANGAN != null && d.ID_PASANGAN.Contains(searchTerm)) ||
                            (d.NAMA_PASANGAN != null && d.NAMA_PASANGAN.Contains(searchTerm)) ||
                            // (d.JK_PASANGAN != null && d.JK_PASANGAN.Contains(searchTerm)) ||
                            // (d.MARITAL_PASANGAN != null && d.MARITAL_PASANGAN.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_PASANGAN != null && d.TPTLAHIR_PASANGAN.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.ToString().Contains(searchTerm)) ||
                            // (d.ID_DEBITUR != null && d.ID_DEBITUR.Contains(searchTerm)) ||
                            (d.NAMA_DEBITUR != null && d.NAMA_DEBITUR.Contains(searchTerm)) ||
                            // (d.JK_DEBITUR != null && d.JK_DEBITUR.Contains(searchTerm)) ||
                            // (d.MARITAL_DEBITUR != null && d.MARITAL_DEBITUR.Contains(searchTerm)) ||
                            // (d.TPTLAHIR_DEBITUR != null && d.TPTLAHIR_DEBITUR.Contains(searchTerm)) ||
                            // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.ToString().Contains(searchTerm)) ||
                            // (d.ALAMAT_DEBITUR != null && d.ALAMAT_DEBITUR.Contains(searchTerm)) ||
                            // (d.RT_DEBITUR != null && d.RT_DEBITUR.Contains(searchTerm)) ||
                            // (d.RW_DEBITUR != null && d.RW_DEBITUR.Contains(searchTerm)) ||
                            // (d.KELURAHAN_DEBITUR != null && d.KELURAHAN_DEBITUR.Contains(searchTerm)) ||
                            // (d.KECAMATAN_DEBITUR != null && d.KECAMATAN_DEBITUR.Contains(searchTerm)) ||
                            // (d.KABUPATEN_DEBITUR != null && d.KABUPATEN_DEBITUR.Contains(searchTerm)) ||
                            // (d.PROVINSI_DEBITUR != null && d.PROVINSI_DEBITUR.Contains(searchTerm)) ||
                            // (d.POS_DEBITUR != null && d.POS_DEBITUR.Contains(searchTerm)) ||
                            // (d.HP_DEBITUR != null && d.HP_DEBITUR.Contains(searchTerm)) ||
                            // (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.ToString().Contains(searchTerm)) ||
                            // (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.ToString().Contains(searchTerm)) ||
                            (d.NOMOR_KONTRAK != null && d.NOMOR_KONTRAK.Contains(searchTerm)) ||
                            // (d.HUTANG_POKOK.HasValue && d.HUTANG_POKOK.Value.ToString().Contains(searchTerm)) ||
                            // (d.NILAI_JAMINAN.HasValue && d.NILAI_JAMINAN.Value.ToString().Contains(searchTerm)) ||
                            // (d.NILAI_BARANG.HasValue && d.NILAI_BARANG.Value.ToString().Contains(searchTerm)) ||
                            (d.JENIS_OBJECT != null && d.JENIS_OBJECT.Contains(searchTerm)) ||
                            // (d.MODEL != null && d.MODEL.Contains(searchTerm)) ||
                            // (d.MERK != null && d.MERK.Contains(searchTerm)) ||
                            // d.TIPE != null && d.TIPE.Contains(searchTerm)) ||
                            // (d.TAHUN.HasValue && d.TAHUN.Value.ToString().Contains(searchTerm)) ||
                            // (d.WARNA != null && d.WARNA.Contains(searchTerm)) ||
                            (d.NOMOR_RANGKA != null && d.NOMOR_RANGKA.Contains(searchTerm)) ||
                            (d.NOMOR_MESIN != null && d.NOMOR_MESIN.Contains(searchTerm)) ||
                            (d.NOMOR_POLISI != null && d.NOMOR_POLISI.Contains(searchTerm)) ||
                            (d.NOMOR_BPKB != null && d.NOMOR_BPKB.Contains(searchTerm)) ||
                            // (d.NAMA_BPKB != null && d.NAMA_BPKB.Contains(searchTerm)) ||
                            // (d.PEMILIK_BPKB != null && d.PEMILIK_BPKB.Contains(searchTerm)) ||
                            // (d.TENOR != null && d.TENOR.Contains(searchTerm)) ||
                            // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.ToString().Contains(searchTerm)) ||
                            // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.ToString().Contains(searchTerm)) ||
                            (d.TYPE_PRODUK != null && d.TYPE_PRODUK.Contains(searchTerm)) ||
                            // (d.WAY_OF_FINANCING != null && d.WAY_OF_FINANCING.Contains(searchTerm)) ||
                            // (d.USER_BY != null && d.USER_BY.Contains(searchTerm)) ||
                            (d.MESSAGES != null && d.MESSAGES.Contains(searchTerm)) ||
                            (d.NAMA_KWITANSI != null && d.NAMA_KWITANSI.Contains(searchTerm))
                    );

                    if (DateTime.TryParseExact(searchTerm, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        query = query.Where(d =>
                                (d.TANGGAL_AKTA.HasValue && d.TANGGAL_AKTA.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AWAL_TENOR.HasValue && d.TANGGAL_AWAL_TENOR.Value.Date == parsedDate.Date) ||
                                // (d.TANGGAL_AKHIR_TENOR.HasValue && d.TANGGAL_AKHIR_TENOR.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_ORDER.HasValue && d.TANGGAL_ORDER.Value.Date == parsedDate.Date) ||
                                (d.TANGGAL_KONTRAK.HasValue && d.TANGGAL_KONTRAK.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_DEBITUR.HasValue && d.TGLLAHIR_DEBITUR.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PASANGAN.HasValue && d.TGLLAHIR_PASANGAN.Value.Date == parsedDate.Date) ||
                                // (d.TGLLAHIR_PEMBERIFIDUSIA.HasValue && d.TGLLAHIR_PEMBERIFIDUSIA.Value.Date == parsedDate.Date||
                                (d.TGL_SERTIFIKAT.HasValue && d.TGL_SERTIFIKAT.Value.Date == parsedDate.Date)
                        );
                    }
                }
            }

            // Sorting
            if (!String.IsNullOrEmpty(Param.SortBy) && !String.IsNullOrEmpty(Param.SortValue))
            {
                var Sort = Param.SortBy;
                var sortProperty = typeof(DataModels).GetProperty(Param.SortBy);
                if (sortProperty != null)
                {
                    if (Param.SortValue == "ASC")
                    {
                        query = query.OrderBy(d => sortProperty.GetValue(d));
                    }
                    else
                    {
                        query = query.OrderByDescending(d => sortProperty.GetValue(d));
                    }
                }
            }
            else
            {
                //query = query.OrderByDescending(d => d.INSERT_DATE).ThenBy(d => d.ID);
                query = query.OrderBy(d => d.ID);
            }


            // Pagination and execution
            var total = await query.CountAsync();
            var dataList = await query
                .Skip(Param.StartTake)
                .Take(Param.PageSize)
                .AsNoTracking()
                .ToListAsync();
            var result = new ResponseModel(ResponseCode.OK, "Success", total, dataList);
            return result;

        }



    }
}
