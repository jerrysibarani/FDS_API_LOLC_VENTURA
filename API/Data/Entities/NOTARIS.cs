using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("NOTARIS", Schema = "public")]
    public class NOTARIS
    {
        [Key]
        public int NOTARIS_ID { get; set; }

        [Required(ErrorMessage = "Notaris Code is required")]
        public string? NOTARIS_CODE { get; set; }

        [Required(ErrorMessage = "Notaris Name is required")]
        public string? NOTARIS_NAME { get; set; }
        public string? SK { get; set; }
        public string? ALAMAT { get; set; }
        public string? RT { get; set; }
        public string? RW { get; set; }

        [Required(ErrorMessage = "Provinsi is required")]
        public string? PROVINSI { get; set; }
        public string? KOTA { get; set; }
        public string? KECAMATAN { get; set; }
        public string? KELURAHAN { get; set; }
        public string? POS { get; set; }
        public bool ISACTIVE { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

        [Required(ErrorMessage = "Client Name is required")]
        public string? CLIENT_CODE { get; set; }
    }
}
