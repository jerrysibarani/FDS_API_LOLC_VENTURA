using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("INVOICE", Schema = "public")]
    public class INVOICE
    {
        [Key]
        public int INVOICE_ID { get; set; }
        public string? INVOICE_NO { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public DateTime INVOICE_DATE { get; set; }
        public int TOTAL_DATA { get; set; }
        public double TOTAL_COST { get; set; }
        public double TOTAL_FEE { get; set; }
        public double TOTAL_TAX { get; set; }

        public byte STATUS_ID { get; set; }
        public string? STATUS_NAME { get; set; }
        public bool ISACTIVE { get; set; }
        public bool ISDELETE { get; set; }
    }
}
