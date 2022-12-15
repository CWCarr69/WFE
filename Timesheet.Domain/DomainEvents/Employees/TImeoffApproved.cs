using Timesheet.Domain;

namespace Timesheet.DomainEvents.Employees
{
    public record TimeoffApprovedEntry(
        string Id, 
        string EmployeeId,
        DateTime RequestDate, 
        int TypeId, 
        double Hours, 
        string Description,
        bool IsSalaried);
        
    public record TimeoffApproved(IEnumerable<TimeoffApprovedEntry> TimeoffEntries) : IDomainEvent;
}
