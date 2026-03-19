namespace Timesheet.Domain.DomainEvents.Employees
{
    public record TimeoffApprovedEntry(
        string Id,
        string EmployeeId,
        DateTime RequestDate,
        int TypeId,
        double Hours,
        string Description,
        bool IsSalaried,
        string? ProfitCenter);

    public record TimeoffApproved(IEnumerable<TimeoffApprovedEntry> TimeoffEntries, bool ForceAction) : IDomainEvent;
}
