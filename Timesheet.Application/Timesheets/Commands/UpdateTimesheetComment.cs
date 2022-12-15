using Timesheet.Application.Shared;

namespace Timesheet.Application.Timesheets.Commands
{
    public class UpdateTimesheetComment : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimesheetId { get; set; }
        public string? EmployeeComment { get; set; }
        public string? ApproverComment { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
