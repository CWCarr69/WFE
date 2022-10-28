namespace Timesheet.Domain.ReadModels
{
    public class WithTotal<T>
    {
        public int TotalItems { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
