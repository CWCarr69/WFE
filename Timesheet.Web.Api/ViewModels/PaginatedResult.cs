namespace Timesheet.Web.Api.ViewModels
{
    public class PaginatedResult<T>
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public long TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; }
        public IDictionary<string, object> OtherData { get; set; } = new Dictionary<string, object>();
    }
}
