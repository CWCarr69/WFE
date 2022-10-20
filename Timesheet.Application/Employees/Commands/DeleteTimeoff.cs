using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class DeleteTimeoff : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimeoffId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.DELETION;

    }
}
