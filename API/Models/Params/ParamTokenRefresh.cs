using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamTokenRefresh
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        public string? RefreshToken { get; set; }
    }
}
