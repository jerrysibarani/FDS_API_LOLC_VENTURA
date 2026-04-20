using System.ComponentModel.DataAnnotations;

namespace API.Models.Params
{
    public class ParamData
    {

        [Required(ErrorMessage = "Insert Date is required. with format : yyyy-MM-dd")]
        [Display(Name = "Insert Date")]
        public DateOnly InsertDate { get; set; }

        [Required(ErrorMessage = "Start Record is required")]
        [Display(Name = "StartTake")]
        public int StartTake { get; set; }

        [Required(ErrorMessage = "Page Size is required")]
        [Display(Name = "PageSize")]
        public int PageSize { get; set; }

        [Display(Name = "ParamSearch")]
        public string? ParamSearch { get; set; }


        [Display(Name = "SortBy")]
        public string? SortBy { get; set; }


        [Display(Name = "SortValue")]
        public string? SortValue { get; set; }


        [Required(ErrorMessage = "Customer Code is required")]
        [Display(Name = "CustomerCode")]
        public string? CustomerCode { get; set; }

    }
}
