using Timesheet.Application.Shared;

namespace Timesheet.Application.Timesheets.Commands
{
    public class FinalizeTimesheet : BaseCommand
    {
        public string TimesheetId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
