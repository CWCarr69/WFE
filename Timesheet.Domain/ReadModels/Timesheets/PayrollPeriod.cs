using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class PayrollPeriod
    {
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimesheetType Type { get; set; }
    }
}
