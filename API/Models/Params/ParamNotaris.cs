using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamNotaris
    {
        [Required(ErrorMessage = "ID is required")]
        [Display(Name = "ID")]
        public int ID { get; set; }



        [Required(ErrorMessage = "Notaris Code is required")]
        [Display(Name = "Notaris Code")]
        public string NOTARIS_CODE { get; set; }
    }
}
