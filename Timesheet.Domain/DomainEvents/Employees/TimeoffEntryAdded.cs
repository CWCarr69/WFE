namespace Timesheet.Domain.DomainEvents.Employees
{
    public record TimeoffEntryAdded(DateTime RequestDate, bool isSalaried) : IDomainEvent;
}