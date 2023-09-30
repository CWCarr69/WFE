namespace Timesheet.Domain.DomainEvents.Employees
{
    public record TimeoffEntryAdded(DateTime RequestDate, bool IsSalaried, bool ForceAction) : IDomainEvent;
}