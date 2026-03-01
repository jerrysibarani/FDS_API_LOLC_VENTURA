namespace API.Models.Params
{
    public class ParamFile
    {
        public int ID { get; set; }
        public required Byte[] Content { get; set; }
        public required string FileName { get; set; }
        public required string CustomerCode { get; set; }
    }
}
