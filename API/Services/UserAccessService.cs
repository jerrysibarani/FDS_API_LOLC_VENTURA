using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.Helpers;
using API.IServices;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;

namespace API.Services
{
    public class UserAccessService(
        UserManager<ApplicationUser> userManager,
        AppDbContext DBContext
    ) : IUserAccessService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _dbContext = DBContext;

        private async Task<ApplicationUser?> GetCurrentUserAsync(Principal UserCurrent)
        {
            return await _userManager.GetUserAsync(UserCurrent.User);
        }
        private IList<string> GetUserRolesAsync(Principal UserCurrent)
        {
            if (UserCurrent?.User is null) return [];

            var roles = UserCurrent.User
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            return roles;

        }
        private async Task<IList<string>> GetUserRolesFromDbByUserIdAsync(string UserCurrent, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(UserCurrent)) return [];
            return await _dbContext.UserRoles
                .Where(x => x.UserId == UserCurrent)
                .Join(
                    _dbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (_, r) => r.Name!
                )
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        private async Task<IList<string>> GetUserRolesFromDbAsync(Principal UserCurrent)
        {
            if (String.IsNullOrEmpty(UserCurrent.UID)) return [];
            var user = await _userManager.FindByIdAsync(UserCurrent.UID);
            if (user is null) return [];
            return await _userManager.GetRolesAsync(user);

            //if (currentUser is null || string.IsNullOrWhiteSpace(CurrentUser.UID))
            //    return [];
            //return await _dbContext.UserRoles
            //    .Where(x => x.UserId == CurrentUser.UID)
            //    .Join(
            //        _dbContext.Roles,
            //        ur => ur.RoleId,
            //        r => r.Id,
            //        (_, r) => r.Name!
            //    )
            //    .AsNoTracking()
            //    .ToListAsync(CnlToken);
        }


        public async Task<List<ACCESSROLES>?> GetAllAccessUser(Principal UserCurrent, CancellationToken cancellationToken = default)
        {
            var roles = GetUserRolesAsync(UserCurrent);
            if (roles.Count == 0) return [];

            return await _dbContext.AccessRoles
                .Where(ar => ar.ISACTIVE && roles.Contains(ar.ROLE_NAME!))
                .Join(_dbContext.Access.Where(ac => ac.ISACTIVE), ar => ar.ACCESS_ID, ac => ac.ACCESS_ID, (ar, _) => ar)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ACCESSROLES?> GetAccessUserByPage(string PageName, Principal UserCurrent, CancellationToken cancellationToken = default)
        {
            var roles = GetUserRolesAsync(UserCurrent);
            if (roles.Count == 0) return null;
            return await _dbContext.AccessRoles
                .Where(ar => ar.ISACTIVE && roles.Contains(ar.ROLE_NAME!))
                .Join(_dbContext.Access.Where(ac => ac.ISACTIVE && ac.ACCESS_CODE == PageName),
                      ar => ar.ACCESS_ID, ac => ac.ACCESS_ID, (ar, _) => ar)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<MenuAccessModels>> GetMenuAccesUser(Principal UserCurrent, CancellationToken cancellationToken = default)
        {
            var roles = GetUserRolesAsync(UserCurrent);
            if (roles.Count == 0) return [];

            var subMenu = await _dbContext.Access
                .Join(_dbContext.AccessRoles, acc => acc.ACCESS_ID, ar => ar.ACCESS_ID, (acc, ar) => new { acc, ar })
                .Where(x => x.acc.ISACTIVE && x.ar.ACCESS_VIEW && x.ar.ISACTIVE && roles.Contains(x.ar.ROLE_NAME!))
                .Select(x => x.acc)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var groupedMainIds = subMenu.Where(x => x.ACCESS_MENU > 0).Select(x => x.ACCESS_MENU).Distinct().ToList();

            var allMain = await _dbContext.Access
                .Where(x => groupedMainIds.Contains(x.ACCESS_ID) && (x.ACCESS_MENU == null || x.ACCESS_MENU < 0) && x.ISACTIVE)
                .OrderBy(x => x.DISPLAY_ORDER)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

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

        public async Task<PersonalProfileModels> GetProfileUser(ApplicationUser Users, CancellationToken cancellationToken = default)
        {
            // Gunakan result.Customer, result.Branch (bisa null), result.Client (bisa null)

            if (Users is null) return new();

            var customerTask = await _dbContext.Customers.Where(x => x.CUSTOMER_CODE == Users.CUSTOMER_CODE).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            var branchTask = await _dbContext.Branches.Where(x => x.BRANCH_CODE == Users.BRANCH_CODE).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            var clientTask = await _dbContext.Clients.Where(x => x.CLIENT_CODE == Users.CLIENT_CODE).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            var rolesTask = await GetUserRolesFromDbByUserIdAsync(Users.Id!, cancellationToken);

            return new PersonalProfileModels
            {
                Id = Users.Id,
                FullName = Users.FullName,
                UserName = Users.UserName,
                Email = Users.Email,
                Address = Users.Address,
                City = Users.City,
                PhoneNumber = Users.PhoneNumber,
                BRANCH_CODE = Users.BRANCH_CODE,
                USER_TYPE = Users.USER_TYPE,
                CLIENT_CODE = Users.CLIENT_CODE,
                CUSTOMER_CODE = Users.CUSTOMER_CODE,
                BRANCH_NAME = branchTask?.BRANCH_NAME,
                CUSTOMER_NAME = customerTask?.CUSTOMER_NAME,
                CLIENT_NAME = clientTask?.CLIENT_NAME,
                IsSuperAdmin = rolesTask.Contains(ConstantaData.RoleSuperAdmin),
                AHU_USERID = Users.AHU_USERID,
            };
        }
    }

}
