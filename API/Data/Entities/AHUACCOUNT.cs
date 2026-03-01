using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("AHUACCOUNT", Schema = "public")]
    public class AHUACCOUNT
    {
        [Key]
        public int AHU_ID { get; set; }

        [Required(ErrorMessage = "Usernama is required")]
        [Display(Name = "Username")]
        public string? AHU_USERID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string? AHU_PASSWORD { get; set; }
        public bool ISACTIVE { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
        public string? CUSTOMER_CODE { get; set; }
        public string? CLIENT_CODE { get; set; }

        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

        public string? PRIVATE_KEY { get; set; }
        public string? PUBLIC_KEY { get; set; }
    }
}
