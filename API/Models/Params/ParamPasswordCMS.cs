using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamPasswordCMS
    {
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [Display(Name = "New Password")]
        public required string NewPassword { get; set; }
    }
}
