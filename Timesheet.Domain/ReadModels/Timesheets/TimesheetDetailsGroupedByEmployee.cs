using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class TimesheetDetailsGroupedByEmployee
    {
        public string TimesheetId { get; set; }
        public string EmployeeId { get; set; }
        public string Fullname { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PayrollPeriod { get; set; }
        public double Total { get; set; }
        public double Overtime { get; set; }
        public TimesheetStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public IEnumerable<EmployeeTimesheetEntry> Details { get; set; }
    }
}