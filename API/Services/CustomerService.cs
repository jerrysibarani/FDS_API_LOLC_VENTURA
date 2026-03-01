using API.Data;
using API.IServices;
using API.Models.Views;
using API.Models;
using API.Data.Models;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using API.Model;
using Newtonsoft.Json;

namespace API.Services
{
    public class CustomerService(
        AppDbContext dbContext
    ) : ICustomerService
    {

        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<PostValueModels>> GetCodeCustomer(Principal currentUser)
        {

            var query = _dbContext.Customers
                    .AsNoTracking()
                    .Where(c => c.ISACTIVE);

            if (currentUser.UserType == ConstantaData.INTERNAL)
            {
                query = query.Where(c => c.CLIENT_CODE == currentUser.ClientCode);
            }

            return await query
                .Select(c => new PostValueModels
                {
                    POST_NAME = c.CUSTOMER_NAME,
                    POST_VALUE = c.CUSTOMER_CODE
                })
                .ToListAsync();

        }
    }
}
