using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Exceptions
{
    public sealed class CannotFinalizeTImesheetException : DomainException
    {
        public CannotFinalizeTImesheetException(string payrollPeriod) 
        : base($"{nameof(TimesheetHeader)}.CannotFinalize", 400, $"{nameof(TimesheetHeader)} ({payrollPeriod}) cannot be finalized yet.")
        {
        }
    }
}
