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
    }
}
