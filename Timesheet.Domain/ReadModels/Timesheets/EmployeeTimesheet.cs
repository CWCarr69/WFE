using System.Security;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Models.Referential;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class EmployeeTimesheet
    {
        public string NextTimesheetId { get; set; }

        public string PreviousTimesheetId { get; set; }

        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimesheetId { get; set; }
        public string PayrollPeriod { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ApproverComment { get; set; }
        public string EmployeeComment { get; set; }
        public TimesheetStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public TimesheetEntryStatus PartialStatus { get; set; }
        public string PartialStatusName => PartialStatus.ToString();
        public string PayrollCode { get; set; }
        public double TotalHours { get; set; }
        public TimesheetType Type { get; set; }
        public IEnumerable<EmployeeTimesheetEntry> Entries { get; set; } = new List<EmployeeTimesheetEntry>();

        public IEnumerable<EmployeeTimesheetEntry> EntriesWithoutTimeoffs => Entries?.Where(t => !t.IsTimeoff) ?? new List<EmployeeTimesheetEntry>();
    }

    public class EmployeeTimesheetWhithHoursPerStatus : EmployeeTimesheet
    {
        public double TotalInProgress { get; set; }
        public double TotalSubmitted { get; set; }
    }
}
