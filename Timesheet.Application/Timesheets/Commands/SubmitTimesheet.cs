using Timesheet.Application.Shared;

namespace Timesheet.Application.Timesheets.Commands
{
    public class SubmitTimesheet : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimesheetId { get; set; }
        public string? Comment { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
