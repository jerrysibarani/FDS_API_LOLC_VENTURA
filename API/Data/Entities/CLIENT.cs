using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("CLIENT", Schema = "public")]
    public class CLIENT
    {
        [Key]
        public int CLIENT_ID { get; set; }


        [Required(ErrorMessage = "Client Code is required")]
        public string? CLIENT_CODE { get; set; }

        [Required(ErrorMessage = "Client Name is required")]
        public string? CLIENT_NAME { get; set; }

        [Required(ErrorMessage = "Telephone is required")]
        public string? TELP { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string? EMAIL { get; set; }

        [Required(ErrorMessage = "Alamat is required")]
        public string? ALAMAT { get; set; }

        [Required(ErrorMessage = "Provinsi is required")]
        public string? PROVINSI { get; set; }

        [Required(ErrorMessage = "Kota is required")]
        public string? KOTA { get; set; }

        [Required(ErrorMessage = "Kode Pos is required")]
        public string? POS { get; set; }

        public bool ISACTIVE { get; set; } = true;

        [Required(ErrorMessage = "Legal PIC is required")]
        public string? LEGAL_PIC { get; set; }

        [Required(ErrorMessage = "Bank Account Name is required")]
        public string? ACCOUNT_NAME { get; set; }

        
    }
}
