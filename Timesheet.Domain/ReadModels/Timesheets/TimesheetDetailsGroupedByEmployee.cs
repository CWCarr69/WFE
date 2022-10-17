namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class TimesheetDetailsGroupedByEmployee
    {
        public string EmployeeId { get; set; }
        public string Fullname { get; set; }
        public string PayrollPeriodNumber { get; set; }
        public string PayrollPeriod { get; set; }
        public double Total { get; set; }
        public double Overtime { get; set; }
        IEnumerable<EmployeeTimesheetEntry> Details { get; set; }
    }
}