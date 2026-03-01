using System.ComponentModel.DataAnnotations;

namespace API.Models.Views
{
    public class AhuAccountModels
    {
        public int AHU_ID { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? AHU_USERID { get; set; }

        public string? AHU_PASSWORD { get; set; }

        public string? CUSTOMER_CODE { get; set; }
        public string? CUSTOMER_NAME { get; set; }
        public string? TIPE { get; set; }
        public string? SUB_TIPE { get; set; }
        public string? TIPE_NAME { get; set; }
        public string? JENIS { get; set; }
        public string? NPWP { get; set; }
        public string? NIK { get; set; }
        public string? SK { get; set; }
        public string? NEGARA_ASAL { get; set; }
        public string? TELP { get; set; }
        public string? EMAIL { get; set; }
        public string? KANTOR_CABANG { get; set; }
        public string? ALAMAT { get; set; }
        public string? RT { get; set; }
        public string? RW { get; set; }
        public string? PROVINSI { get; set; }
        public string? KOTA { get; set; }
        public string? KECAMATAN { get; set; }
        public string? KELURAHAN { get; set; }
        public string? POS { get; set; }

    }
}
