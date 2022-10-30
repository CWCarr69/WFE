namespace Timesheet.Domain.DomainEvents
{
    public record TimesheetFinalized(string payrollPeriod) : IDomainEvent;
}
