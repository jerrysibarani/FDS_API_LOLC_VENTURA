using API.Data;
using API.Data.Models;
using API.Helpers;
using API.IServices;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Model;
using Newtonsoft.Json;

namespace API.Services
{
    public class NotarisService(
        AppDbContext dbContext
    ) : INotarisService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IReadOnlyList<PostValueModels>> GetCodeNotaris(Principal UserCurrent, CancellationToken cancellationToken = default)
        {

            var query = _dbContext.Notaris
                    .Where(c => c.ISACTIVE)
                    .AsNoTracking()
                    .AsQueryable();

            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(c => c.CLIENT_CODE == UserCurrent.ClientCode);
            }

            return await query
                .Select(c => new PostValueModels
                {
                    POST_NAME = c.NOTARIS_NAME,
                    POST_VALUE = c.NOTARIS_CODE
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ChangeStatusNotaris(Principal UserCurrent, string Id, CancellationToken cancellationToken = default)
        {
            var notaris = _dbContext.Notaris.Where(x => x.NOTARIS_CODE!.Equals(Id)).AsQueryable();
            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                notaris = notaris.Where(x => x.CLIENT_CODE == UserCurrent.ClientCode);
            }
            var notar = await notaris.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (notar == null)
            {
                return false;
            }
            else
            {
                var now = DateTime.UtcNow;

                // 1️⃣ Deactivate semua notaris lain untuk client
                await _dbContext.Notaris
                    .Where(x => x.CLIENT_CODE == notar.CLIENT_CODE
                             && x.NOTARIS_ID != notar.NOTARIS_ID
                             && x.ISACTIVE)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.ISACTIVE, false)
                        .SetProperty(x => x.MODIFIED_BY, UserCurrent.Email)
                        .SetProperty(x => x.MODIFIED_DATE, now),
                        cancellationToken);

                // 2️⃣ Activate notaris terpilih
                await _dbContext.Notaris
                    .Where(x => x.NOTARIS_ID == notar.NOTARIS_ID)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.ISACTIVE, true)
                        .SetProperty(x => x.MODIFIED_BY, UserCurrent.Email)
                        .SetProperty(x => x.MODIFIED_DATE, now),
                        cancellationToken);

                //var nota = await _dbContext.Notaris
                //                .Where(x => x.CLIENT_CODE == notar.CLIENT_CODE && !x.NOTARIS_ID.Equals(notar.NOTARIS_ID))
                //                .ToListAsync(cancellationToken);
                //if (nota != null && nota.Count > 0)
                //{
                //    nota.ForEach(x =>
                //    {
                //        x.ISACTIVE = false;
                //        x.MODIFIED_BY = UserCurrent.Email;
                //        x.MODIFIED_DATE = DateTime.Now;
                //    });
                //    _dbContext.Notaris.UpdateRange(nota.ToList());
                //}
                //notar.ISACTIVE = true;
                //notar.MODIFIED_BY = UserCurrent.Email;
                //notar.MODIFIED_DATE = DateTime.Now;
                //_dbContext.Notaris.Update(notar);
                //await _dbContext.SaveChangesAsync(cancellationToken); 
                _dbContext.ChangeTracker.Clear();
                return true;
            }
        }

    }
}
