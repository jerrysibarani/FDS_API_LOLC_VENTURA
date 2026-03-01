using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("ACCESS", Schema = "public")]
    public class ACCESS
    {

        [Key]
        public int ACCESS_ID { get; set; }

        [Required(ErrorMessage = "Access Code is required")]
        [Display(Name = "Access Code")]
        public string? ACCESS_CODE { get; set; }

        [Required(ErrorMessage = "Access Name is required")]
        [Display(Name = "Access Name")]
        public string? ACCESS_NAME { get; set; }

        public int? ACCESS_MENU { get; set; }

        public string? ACCESS_ICON { get; set; }

        public bool ISACTIVE { get; set; }

        public int? DISPLAY_ORDER { get; set; }

        public bool ISSUPER_ADMIN { get; set; }
        public bool ISINTERNAL { get; set; }
        public bool ISHIDE { get; set; }

    }
}

