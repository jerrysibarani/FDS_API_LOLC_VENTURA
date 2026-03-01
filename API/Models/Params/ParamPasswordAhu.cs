using API.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamPasswordAhu
    {

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public required string Password { get; set; }


        [Required(ErrorMessage = "New Password is required")]
        [Display(Name = "New Password")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "User AHU is required")]
        [Display(Name = "AHU_USERID")]
        public required string AHU_USERID { get; set; }

    }
}
