using Timesheet.Domain;

namespace Timesheet.DomainEvents.Employee
{
    public record TimeoffWorkflowChanged(
        string EmployeeId, string SupervisorId, string ManagerId, string Action, string ObjectId
    ) : IDomainEvent;
}
