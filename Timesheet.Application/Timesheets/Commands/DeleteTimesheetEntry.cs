using Timesheet.Application.Shared;

namespace Timesheet.Application.Timesheets.Commands
{
    public class DeleteTimesheetEntry : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimesheetId { get; set; }
        public string TimesheetEntryId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.DELETION;

    }
}
