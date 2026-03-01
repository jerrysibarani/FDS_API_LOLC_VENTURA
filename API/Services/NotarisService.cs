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

        public async Task<List<PostValueModels>> GetCodeNotaris(Principal currentUser)
        {

            var query = _dbContext.Notaris
                    .AsNoTracking()
                    .Where(c => c.ISACTIVE);

            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(c => c.CLIENT_CODE == currentUser.ClientCode);
            }

            return await query
                .Select(c => new PostValueModels
                {
                    POST_NAME = c.NOTARIS_NAME,
                    POST_VALUE = c.NOTARIS_CODE
                })
                .ToListAsync();
        }

        public async Task<bool> ChangeStatusNotaris(Principal currentUser, string id)
        {
            var notaris = _dbContext.Notaris.Where(x => x.NOTARIS_CODE!.Equals(id)).AsQueryable();
            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                notaris = notaris.Where(x => x.CLIENT_CODE == currentUser.ClientCode);
            }
            var notar = await notaris.FirstOrDefaultAsync();
            if (notar == null)
            {
                return false;
            }
            else
            {
                var nota = await _dbContext.Notaris
                                .Where(x => x.CLIENT_CODE == notar.CLIENT_CODE && !x.NOTARIS_ID.Equals(notar.NOTARIS_ID))
                                .ToListAsync();
                if (nota != null && nota.Count > 0)
                {
                    nota.ForEach(x =>
                    {
                        x.ISACTIVE = false;
                        x.MODIFIED_BY = currentUser.Email;
                        x.MODIFIED_DATE = DateTime.Now;
                    });
                    _dbContext.Notaris.UpdateRange(nota.ToList());
                }
                notar.ISACTIVE = true;
                notar.MODIFIED_BY = currentUser.Email;
                notar.MODIFIED_DATE = DateTime.Now;
                _dbContext.Notaris.Update(notar);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

    }
}
