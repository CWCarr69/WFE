namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetHeader : Entity
    {
        public TimesheetHeader(string id,
            string payrollPeriod,
            DateTime startDate,
            DateTime endDate,
            TimesheetStatus status) : base(id)
        {
            PayrollPeriod = payrollPeriod;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
        }

        public string PayrollPeriod { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TimesheetStatus Status { get; private set; }
        public virtual ICollection<TimesheetEntry> TimesheetEntries { get; private set; } = new List<TimesheetEntry>();
    }
}
