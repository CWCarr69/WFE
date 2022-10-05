using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class ModifyApprover : ICommand
    {
        public string EmployeeId { get; set; }
        public string PrimaryApproverId { get; set; }
        public string SecondaryApproverId { get; set; }
        public CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
