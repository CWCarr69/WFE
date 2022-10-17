using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeWithTimeStatus
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimeoffId { get; set; }
        public string TimesheetId { get; set; }
        public int LastTimeoffStatus { get; set; }
        public string LastTimeoffStatusString => ((TimeoffStatus)LastTimeoffStatus).ToString();
        public int LastTimesheetStatus { get; set; }
        public string LastTimesheetStatusString => ((TimesheetStatus)LastTimesheetStatus).ToString();

    }
}
