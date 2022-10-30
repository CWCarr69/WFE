using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class AllEmployeesTimesheet
    {
        public string PayrollPeriod { get; set; }
        public TimesheetStatus Status { get; set; }
        public bool IsFinalized => Status == TimesheetStatus.FINALIZED;
        public IEnumerable<TimesheetEntryDetails> Entries { get; set; }
    }
}
