namespace API.Model
{
    public class ResponseModel
    {
        public ResponseModel(ResponseCode _responseCode, string _responseMessage, int _total, object _data)
        {
            ResponseCode = _responseCode;
            ResponseMessage = _responseMessage;
            Total = _total;
            Data = _data;
        }
        public ResponseCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int Total { get; set; }

        public object Data { get; set; }
    }
}
