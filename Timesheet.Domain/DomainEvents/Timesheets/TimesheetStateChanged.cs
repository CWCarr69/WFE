using Timesheet.Domain;

namespace Timesheet.DomainEvents.Timesheets
{
    public record TimesheetStateChanged(
        string EmployeeId, string PrimaryApproverId, string SecondaryApproverId, string Action, string ObjectId
    ) : IDomainEvent;
}
