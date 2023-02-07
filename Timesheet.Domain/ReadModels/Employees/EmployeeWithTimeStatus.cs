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
        public double VacationBalance { get; set; }
        public double PersonalBalance { get; set; }
        public int LastTimeoffStatus { get; set; }
        public string LastTimeoffEntryId { get; set; }
        public DateTime LastTimeoffRequestDate { get; set; }
        public string LastTimeoffStatusString => ((TimeoffStatus)LastTimeoffStatus).ToString();
        public bool IsLastTimeoffRequireApproval { get; set;  }
        public int LastTimesheetStatus { get; set; }
        public string LastTimesheetPayrollPeriod { get; set; }
        public DateTime LastTimesheetWorkDate { get; set; }
        public string LastTimesheetStatusString => ((TimesheetStatus)LastTimesheetStatus).ToString();

    }
}
