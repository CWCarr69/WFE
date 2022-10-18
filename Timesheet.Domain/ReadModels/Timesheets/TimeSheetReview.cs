namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class TimesheetReview
    {
        public long TotalItems { get; set; }
        public double TotalQuantity { get; set; }
        public IEnumerable<TimesheetDetailsGroupedByEmployee> DetailsByEmployee { get; set; }
    }
}
