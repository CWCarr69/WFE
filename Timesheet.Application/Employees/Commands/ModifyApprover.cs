using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class ModifyApprover : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string PrimaryApproverId { get; set; }
        public string SecondaryApproverId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
