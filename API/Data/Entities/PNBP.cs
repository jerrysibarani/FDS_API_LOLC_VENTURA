using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("PNBP", Schema = "public")]
    public class PNBP
    {
        [Key]
        [Required(ErrorMessage = "PNBP ID is required")]
        public int PNBP_ID { get; set; }

        public double MIN_VALUE { get; set; }

        public double MAX_VALUE { get; set; }

        public double PRICE { get; set; }

        public int ORDER_BY { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public bool ISACTIVE { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
