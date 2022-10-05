namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeWithTimeStatus
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimeoffId { get; set; }
        public string TimesheetId { get; set; }
        public string LastTimeoffStatus { get; set; }
        public string LastTimesheetStatus { get; set; }
    }
}
