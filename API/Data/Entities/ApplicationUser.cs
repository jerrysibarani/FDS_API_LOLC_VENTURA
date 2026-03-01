using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? USER_TYPE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? AHU_USERID { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
