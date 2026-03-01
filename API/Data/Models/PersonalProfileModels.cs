namespace API.Data.Models
{
    public class PersonalProfileModels
    {
        public string? Id { get; set; } // IdentityUser's Id
        public string? UserName { get; set; } // IdentityUser's UserName
        public string? Email { get; set; } // IdentityUser's Email
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }

        public bool? IsSuperAdmin { get; set; }
        public string? USER_TYPE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? BRANCH_NAME { get; set; }
        public string? CUSTOMER_NAME { get; set; }
        public string? CLIENT_NAME { get; set; }
        public string? AHU_USERID { get; set; }
    }
}
