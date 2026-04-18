using API.Data.Entities;
using System.Security.Claims;

namespace API.Models
{
    public class Principal
    {
        public string? UID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? UserType { get; set; }
        public string? ClientCode { get; set; }
        public string? CustomerCode { get; set; }
        public bool IsSuperAdmin { get; set; }

        // ✅ Simpan ClaimsPrincipal (opsional, tapi penting untuk role)
        public ClaimsPrincipal User { get; init; } = default!;

    }
}
