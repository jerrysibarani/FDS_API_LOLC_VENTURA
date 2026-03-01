using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class RolesUserModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Role Name is required.")]
        public string? RoleId { get; set; }
    }
}
