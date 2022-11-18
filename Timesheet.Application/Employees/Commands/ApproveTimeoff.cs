using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class ApproveTimeoff : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimeoffId { get; set; }
        public string? Comment { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
