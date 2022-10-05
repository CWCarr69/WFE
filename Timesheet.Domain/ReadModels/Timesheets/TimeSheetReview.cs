namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class TimeSheetReview
    {
        public string DepartmentId { get; set; }
        public string Department { get; set; }
        public double TotalQuantity { get; set; }
        public IEnumerable<TimesheetDetailsGroupedByEmployee> DetailsByEmployee { get; set; }
    }
}
