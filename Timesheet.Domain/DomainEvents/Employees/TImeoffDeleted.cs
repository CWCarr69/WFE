namespace Timesheet.Domain.DomainEvents.Employees
{
    public record TimeoffDeletedEntry(
        string EmployeeId,
        int TypeId,
        double Hours);

    public record TimeoffDeleted(TimeoffDeletedEntry timeoffDeleted, bool ForceAction) : IDomainEvent;
}
