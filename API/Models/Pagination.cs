namespace API.Model
{
    public class PagedResult<T>
    {
        public IList<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    public class QueryFilter
    {
        public IList<FilterDescriptor> Descriptors { get; set; } = new List<FilterDescriptor>();
    }

    public class FilterDescriptor
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class SortDescriptor
    {
        public string Id { get; set; }
        public bool Desc { get; set; }
    }

    public class PagedQuery
    {
        public IList<FilterDescriptor>? filters { get; set; }
        public IList<SortDescriptor>? sortBy { get; set; }
    }
}
