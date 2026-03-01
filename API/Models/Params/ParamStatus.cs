using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamStatus
    {
        [Required(ErrorMessage = "ID is required")]
        [Display(Name = "ID")]
        public int ID { get; set; }
        public string? NOTARIS_CODE { get; set; }
        public string? NO_VOUCHER { get; set; }
        public string? MESSAGES { get; set; }
    }
}
