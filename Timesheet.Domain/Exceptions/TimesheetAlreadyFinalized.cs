using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Exceptions
{
    public sealed class TimesheetAlreadyFinalized : DomainException
    {
        public TimesheetAlreadyFinalized(string message, string payrollPeriod, string start, string end) 
        : base($"{nameof(TimesheetHeader)}.AlreadyFinalized", 400, $"{message}. Payroll period {payrollPeriod}({start}-{end}) is already finalized.")
        {
        }
    }
}
