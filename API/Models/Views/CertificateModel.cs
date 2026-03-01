namespace API.Models.Views
{
    public class CertificateModel
    {
        public int ID { get; set; }
        public string? CLIENT_CODE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? NO_VOUCHER { get; set; }
        public string? NO_REGISTRASI { get; set; }
        public string? NO_SERTIFIKAT { get; set; }
        public string? NO_PEMBIAYAAN { get; set; }
        public string? NOMOR_AKTA { get; set; }
        public DateTime? TANGGAL_AKTA { get; set; }
        public string? NAMA_PEMBERIFIDUSIA { get; set; }
        public string? NPWP_PEMBERIFIDUSIA { get; set; }
        public string? NAMA_PENERIMAFIDUSIA { get; set; }
        public string? NPWP_PENERIMAFIDUSIA { get; set; }
        public string? CODE_PENERIMAFIDUSIA { get; set; }
        public string? WILAYAH { get; set; }
        public string? NOTARIS_CODE { get; set; }
        public string? NOTARIS_NAME { get; set; }
        public bool StatusCertificate { get; set; }
        public string? NameFileCertificate { get; set; }
        public string? WaktuDaftar { get; set; }
        public string? TypeFidusia { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? BRANCH_NAME { get; set; }

        public double? NILAI_JAMINAN { get; set; }
        public double? PRICE_PNBP { get; set; }
        public double? FEE_COST { get; set; }
        public double? TAX_COST { get; set; }

    }
}
