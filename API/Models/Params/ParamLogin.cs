using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamLogin
    {
        [Required]
        [StringLength(30, ErrorMessage = "Maximal 30 characters")]
        public string? Username { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Maximal 50 characters")]
        public string? Password { get; set; }
    }
}
