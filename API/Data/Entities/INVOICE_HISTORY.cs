using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("INVOICE_HISTORY", Schema = "public")]
    public class INVOICE_HISTORY
    {
        [Key]
        public int HISTORY_ID { get; set; }
        public int INVOICE_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string? CREATED_BY { get; set; }
        public byte STATUS_ID { get; set; }
        public string? STATUS_NAME { get; set; }
    }
}
