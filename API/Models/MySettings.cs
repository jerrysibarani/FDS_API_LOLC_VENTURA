namespace API.Models
{
    public class MySettings
    {
        public string? ApiUrl { get; set; }
        public string? Environment { get; set; }
        public bool IsEncryption { get; set; }
        public string? DocumentsFiles { get; set; }
        public int BatchList { get; set; }
    }
}
