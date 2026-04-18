using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("HISTORY_CERTIFICATE", Schema = "public")]
    public class HISTORY_CERTIFICATE
    {
        public int ID { get; set; }
        public int DOCUMENT_ID { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? NO_VOUCHER { get; set; }
        public string? NO_REGISTRASI { get; set; }
        public string? NO_SERTIFIKAT { get; set; }
        public string? NO_PEMBIAYAAN { get; set; }

        public DateTime? WAKTU_DAFTAR { get; set; }
        public DateTime? TANGGAL_AKTA { get; set; }
        public string? NOMOR_AKTA { get; set; }
        public string? NOTARIS_CODE { get; set; }
        public string? NOTARIS_NAME { get; set; }
        public string? NAMA_PEMBERIFIDUSIA { get; set; }
        public string? NPWP_PEMBERIFIDUSIA { get; set; }
        public string? NAMA_PENERIMAFIDUSIA { get; set; }
        public string? NPWP_PENERIMAFIDUSIA { get; set; }
        public string? CODE_PENERIMAFIDUSIA { get; set; }
        public string? WILAYAH { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? BRANCH_NAME { get; set; }
        public string? TYPE_FIDUSIA { get; set; }
        public string? URL_FIDUSIA { get; set; }
        public string? AHU_USERID { get; set; }
        public bool STATUS_CERTIFICATE { get; set; }
        public string? PATH_CERTIFICATE { get; set; }
        public string? FILENAME_CERTIFICATE { get; set; }

        public double? NILAI_JAMINAN { get; set; }
        public double? PRICE_PNBP { get; set; }
        public double? FEE_COST { get; set; }
        public double? TAX_COST { get; set; }

        public bool ISMATCH { get; set; }
        public int BATCH_NUMBER { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

    }
}
