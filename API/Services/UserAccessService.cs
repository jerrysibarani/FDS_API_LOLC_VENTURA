using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.Helpers;
using API.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Services
{
    public class UserAccessService(
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor
    ) : IUserAccessService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;

        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return CurrentUser is null ? null : await _userManager.GetUserAsync(CurrentUser);
        }

        private async Task<IList<string>> GetUserRolesAsync()
        {
            var user = await GetCurrentUserAsync();
            return user is null ? [] : await _userManager.GetRolesAsync(user);
        }

        private async Task<IList<string?>> GetUserRolesFromDbAsync()
        {
            var userId = CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return [];


            return await _dbContext.UserRoles
                .Where(x => x.UserId == userId)
                .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsRoleSuperAdmin()
        {
            var roles = await GetUserRolesAsync();
            return roles.Contains(ConstantaData.RoleSuperAdmin);
        }

        public async Task<List<ACCESSROLES>?> GetAllAccessUser()
        {
            var roles = await GetUserRolesAsync();
            if (roles.Count == 0) return [];

            return await _dbContext.AccessRoles
                .Where(ar => ar.ISACTIVE && roles.Contains(ar.ROLE_NAME!))
                .Join(dbContext.Access.Where(ac => ac.ISACTIVE), ar => ar.ACCESS_ID, ac => ac.ACCESS_ID, (ar, _) => ar)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ACCESSROLES?> GetAccessUserByPage(string pageName)
        {
            var roles = await GetUserRolesAsync();
            if (roles.Count == 0) return null;

            return await _dbContext.AccessRoles
                .Where(ar => ar.ISACTIVE && roles.Contains(ar.ROLE_NAME!))
                .Join(dbContext.Access.Where(ac => ac.ISACTIVE && ac.ACCESS_CODE == pageName),
                      ar => ar.ACCESS_ID, ac => ac.ACCESS_ID, (ar, _) => ar)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<MenuAccessModels>> GetMenuAccesUser()
        {
            var roles = await GetUserRolesFromDbAsync();
            if (roles.Count == 0) return [];

            var subMenu = await _dbContext.Access
                .Join(dbContext.AccessRoles, acc => acc.ACCESS_ID, ar => ar.ACCESS_ID, (acc, ar) => new { acc, ar })
                .Where(x => x.acc.ISACTIVE && x.ar.ACCESS_VIEW && x.ar.ISACTIVE && roles.Contains(x.ar.ROLE_NAME!))
                .Select(x => x.acc)
                .AsNoTracking()
                .ToListAsync();

            var groupedMainIds = subMenu.Where(x => x.ACCESS_MENU > 0).Select(x => x.ACCESS_MENU).Distinct().ToList();

            var allMain = await dbContext.Access
                .Where(x => groupedMainIds.Contains(x.ACCESS_ID) && (x.ACCESS_MENU == null || x.ACCESS_MENU < 0) && x.ISACTIVE)
                .OrderBy(x => x.DISPLAY_ORDER)
                .AsNoTracking()
                .ToListAsync();

            var singleMenu = subMenu.Where(x => x.ACCESS_MENU == 0).OrderBy(x => x.DISPLAY_ORDER).ToList();

            var response = new List<MenuAccessModels>();

            response.AddRange(singleMenu.Select(sgl => new MenuAccessModels { MainMenu = sgl, SubMenus = null }));

            foreach (var main in allMain)
            {
                var subMenus = subMenu.Where(x => x.ACCESS_MENU == main.ACCESS_ID).OrderBy(x => x.DISPLAY_ORDER).ToList();
                response.Add(new MenuAccessModels { MainMenu = main, SubMenus = subMenus });
            }

            return response;
        }

        public async Task<PersonalProfileModels> GetProfileUser()
        {
            // Gunakan result.Customer, result.Branch (bisa null), result.Client (bisa null)

            var user = await GetCurrentUserAsync();
            if (user is null) return new();

            var customerTask = await _dbContext.Customers.Where(x => x.CUSTOMER_CODE == user.CUSTOMER_CODE).AsNoTracking().FirstOrDefaultAsync();
            var branchTask = await _dbContext.Branches.Where(x => x.BRANCH_CODE == user.BRANCH_CODE).AsNoTracking().FirstOrDefaultAsync();
            var clientTask = await _dbContext.Clients.Where(x => x.CLIENT_CODE == user.CLIENT_CODE).AsNoTracking().FirstOrDefaultAsync();

            var rolesTask = GetUserRolesAsync();


            return new PersonalProfileModels
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                BRANCH_CODE = user.BRANCH_CODE,
                USER_TYPE = user.USER_TYPE,
                CLIENT_CODE = user.CLIENT_CODE,
                CUSTOMER_CODE = user.CUSTOMER_CODE,
                BRANCH_NAME = branchTask?.BRANCH_NAME,
                CUSTOMER_NAME = customerTask?.CUSTOMER_NAME,
                CLIENT_NAME = clientTask?.CLIENT_NAME,
                IsSuperAdmin = rolesTask.Result.Contains(ConstantaData.RoleSuperAdmin),
                AHU_USERID = user.AHU_USERID,
            };
        }
    }

}
