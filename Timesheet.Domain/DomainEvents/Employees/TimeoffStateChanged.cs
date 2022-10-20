using Timesheet.Domain;

namespace Timesheet.DomainEvents.Employees
{
    public record TimeoffStateChanged(
        string EmployeeId, string PrimaryApproverId, string SecondaryApproverId, string Action, string ObjectId
    ) : IDomainEvent;
}
