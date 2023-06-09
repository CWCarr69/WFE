namespace Timesheet.Domain.DomainEvents.Timesheets
{
    public record TimesheetExceptionAdded(string EmployeeId, string EntryId) : IDomainEvent;
}
