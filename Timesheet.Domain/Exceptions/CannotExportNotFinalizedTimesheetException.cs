using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Exceptions
{
    public sealed class CannotExportNotFinalizedTimesheetException : DomainException
    {
        public CannotExportNotFinalizedTimesheetException(string payrollPeriod) 
        : base($"{nameof(TimesheetHeader)}.CannotExportNonFinalized", 400, $"{nameof(TimesheetHeader)} ({payrollPeriod}) cannot be exported to external.")
        {
        }
    }
}
