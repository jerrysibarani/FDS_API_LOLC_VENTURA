using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("CUSTOMER", Schema = "public")]
    public class CUSTOMER
    {
        [Key]
        public int CUSTOMER_ID { get; set; }

        [Required(ErrorMessage = "Customer Code is required")]
        public string? CUSTOMER_CODE { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        public string? CUSTOMER_NAME { get; set; }


        [Required(ErrorMessage = "Client Code is required")]
        public string? CLIENT_CODE { get; set; }


        [Required(ErrorMessage = "Type Name is required")]
        public string? TIPE { get; set; }

        [Required(ErrorMessage = "Sub Type Name is required")]
        public string? SUB_TIPE { get; set; }

        //[Required(ErrorMessage = "Type Name is required")]
        public string? TIPE_NAME { get; set; }

        //[Required(ErrorMessage = "Category Name is required")]
        public string? JENIS { get; set; }


        [Required(ErrorMessage = "NPWP is required")]
        public string? NPWP { get; set; }

        public string? NIK { get; set; }


        [Required(ErrorMessage = "SK is required")]
        public string? SK { get; set; }

        public string? NEGARA_ASAL { get; set; }

        [Required(ErrorMessage = "Telephone is required")]
        public string? TELP { get; set; }


        [Required(ErrorMessage = "Email is required")]
        public string? EMAIL { get; set; }

        public string? KANTOR_CABANG { get; set; }


        [Required(ErrorMessage = "Alamat is required")]
        public string? ALAMAT { get; set; }

        public string? RT { get; set; }

        public string? RW { get; set; }


        [Required(ErrorMessage = "Provinsi is required")]
        public string? PROVINSI { get; set; }


        [Required(ErrorMessage = "Kota is required")]
        public string? KOTA { get; set; }

        [Required(ErrorMessage = "Kecamatan is required")]
        public string? KECAMATAN { get; set; }

        [Required(ErrorMessage = "Kelurahan is required")]
        public string? KELURAHAN { get; set; }

        [Required(ErrorMessage = "Kode Pos is required")]
        public string? POS { get; set; }

        public bool ISACTIVE { get; set; } = true;


        
    }
}
