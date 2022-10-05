namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class EmployeeTimesheet
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimesheetId { get; set; }
        public double PayrollPeriod { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
