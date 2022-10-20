using Timesheet.Application.Shared;

namespace Timesheet.Application.Timesheets.Commands
{
    public class RejectTimesheet : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimesheetId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
