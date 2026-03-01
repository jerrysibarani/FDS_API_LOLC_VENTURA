using API.Data;
using API.Helpers;
using API.Utils;
using API.IServices;
using API.Models;
using API.Models.Params;
using API.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AHUService(
        AppDbContext dbContext
    ) : IAHUService
    {


        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<AhuAccountModels>> GetAhuAccounts(Principal currentUser)
        {
            var query = _dbContext.AhuAccounts
                .Where(a => a.ISACTIVE)
                .Join(
                    _dbContext.Customers.Where(p => p.ISACTIVE),
                    a => a.CUSTOMER_CODE,
                    p => p.CUSTOMER_CODE,
                    (a, p) => new { a, p }
                )
                .Select(x => new AhuAccountModels
                {
                    AHU_ID = x.a.AHU_ID,
                    AHU_USERID = x.a.AHU_USERID,
                    AHU_PASSWORD = x.a.AHU_PASSWORD,
                    CUSTOMER_CODE = x.a.CUSTOMER_CODE,
                    CLIENT_CODE = x.a.CLIENT_CODE,
                    CUSTOMER_NAME = x.p.CUSTOMER_NAME,
                    TIPE = x.p.TIPE,
                    SUB_TIPE = x.p.SUB_TIPE,
                    TIPE_NAME = x.p.TIPE_NAME,
                    JENIS = x.p.JENIS,
                    NPWP = x.p.NPWP,
                    NIK = x.p.NIK,
                    SK = x.p.SK,
                    NEGARA_ASAL = x.p.NEGARA_ASAL,
                    TELP = x.p.TELP,
                    EMAIL = x.p.EMAIL,
                    KANTOR_CABANG = x.p.KANTOR_CABANG,
                    ALAMAT = x.p.ALAMAT,
                    RT = x.p.RT,
                    RW = x.p.RW,
                    PROVINSI = x.p.PROVINSI,
                    KOTA = x.p.KOTA,
                    KECAMATAN = x.p.KECAMATAN,
                    KELURAHAN = x.p.KELURAHAN,
                    POS = x.p.POS
                })
                .AsNoTracking();

            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }

            return await query.ToListAsync();
        }


        public async Task<bool> ChangePasswordAHU(Principal currentUser, ParamPasswordAhu param)
        {
            string prmOldPass = EncrDecrRsa.DecryptionRSA(param.Password);
            var ahuAccounts = await _dbContext.AhuAccounts.Where(x => x.ISACTIVE.Equals(true) && x.AHU_USERID == param.AHU_USERID).ToListAsync();
            if (ahuAccounts.Count() > 0)
            {
                string? encOldPass = null;
                foreach (var ahu in ahuAccounts)
                {
                    if (string.IsNullOrEmpty(encOldPass))
                    {
                        encOldPass = EncrDecrRsa.DecryptionRSA(param.Password);
                    }
                    if (!string.IsNullOrEmpty(encOldPass) && encOldPass == prmOldPass)
                    {
                        ahu.AHU_PASSWORD = param.NewPassword;
                        ahu.MODIFIED_BY = currentUser.Email;
                        ahu.MODIFIED_DATE = DateTime.UtcNow;
                    }
                }
                _dbContext.AhuAccounts.UpdateRange(ahuAccounts);
                await _dbContext.SaveChangesAsync();
                _dbContext.ChangeTracker.Clear();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
