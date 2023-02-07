using Timesheet.Application.Shared;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Timesheets.Commands
{
    public class AddTimesheetException : BaseCommand
    {
        public string TimesheetEntryId { get; set; }
        public string TimesheetId { get; set; }
        public string EmployeeId { get; set; }
        public bool IsHoliday { get; set; }
        public override CommandActionType ActionType() => CommandActionType.DELETION;
    }
}
