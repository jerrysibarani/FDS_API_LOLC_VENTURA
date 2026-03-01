using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("BRANCH", Schema = "public")]
    public class BRANCH
    {
        [Key]
        public int BRANCH_ID { get; set; }


        [Required(ErrorMessage = "Branch Code is required")]
        [Display(Name = "Branch Code")]
        public string? BRANCH_CODE { get; set; }


        [Required(ErrorMessage = "Branch Name is required")]
        [Display(Name = "Branch Name")]
        public string? BRANCH_NAME { get; set; }


        [Required(ErrorMessage = "Customer Name is required")]
        [Display(Name = "Customer")]
        public string? CUSTOMER_CODE { get; set; }

        public bool ISACTIVE { get; set; }

    }
}
