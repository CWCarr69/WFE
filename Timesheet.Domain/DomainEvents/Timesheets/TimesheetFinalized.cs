namespace Timesheet.Domain.DomainEvents
{
    public record TimesheetFinalized(string timesheetId) : IDomainEvent;
}
