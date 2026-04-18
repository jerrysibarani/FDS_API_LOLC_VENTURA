using API.Data;
using API.IServices;
using API.Models;
using API.Data.Models;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CustomerService(
        AppDbContext dbContext
    ) : ICustomerService
    {

        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IReadOnlyList<PostValueModels>> GetCodeCustomer(Principal UserCurrent, CancellationToken cancellationToken = default)
        {

            var query = _dbContext.Customers
                    .Where(c => c.ISACTIVE);

            if (UserCurrent.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(c => c.CLIENT_CODE == UserCurrent.ClientCode);
            }

            return await query
                .Select(c => new PostValueModels
                {
                    POST_NAME = c.CUSTOMER_NAME,
                    POST_VALUE = c.CUSTOMER_CODE
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        }
    }
}
