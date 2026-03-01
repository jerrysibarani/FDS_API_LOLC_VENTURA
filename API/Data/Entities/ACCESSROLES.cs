using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("ACCESSROLES", Schema = "public")]
    public class ACCESSROLES
    {
        [Key]
        public int ACCESS_ROLE_ID { get; set; }

        [Required(ErrorMessage = "Access Name is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Access Name is required.")]
        public int ACCESS_ID { get; set; }

        [Required(ErrorMessage = "Role Name is required.")]
        public string? ROLE_NAME { get; set; }
        public bool ACCESS_VIEW { get; set; }
        public bool ACCESS_ADD { get; set; }
        public bool ACCESS_EDIT { get; set; }
        public bool ACCESS_DELETE { get; set; }
        public bool ISACTIVE { get; set; }

        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
