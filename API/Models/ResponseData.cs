namespace API.Model
{
    public class ResponseData<T>
    {
            public IList<T> Data { get; set; } = new List<T>();

            public string? ResponseMessage { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
            public int PageNumber { get; set; }

    }
}
