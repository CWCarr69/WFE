namespace Timesheet.Domain.DomainEvents.Timesheets
{
    public record TimesheetRejected( string EmployeeId, List<DateTime> Dates ) : IDomainEvent;
}
