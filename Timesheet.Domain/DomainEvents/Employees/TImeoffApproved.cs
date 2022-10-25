using Timesheet.Domain;

namespace Timesheet.DomainEvents.Employees
{
    public record TimeoffApproved(
        string Id, 
        string EmployeeId,
        DateTime RequestDate, 
        string Type, 
        double Hours, 
        string Description,
        bool IsSalaried) 
        : IDomainEvent;
}
