using Timesheet.Domain;

namespace Timesheet.DomainEvents.Timesheets
{
    public record TimesheetRejected( string EmployeeId, List<DateTime> Dates ) : IDomainEvent;
}
