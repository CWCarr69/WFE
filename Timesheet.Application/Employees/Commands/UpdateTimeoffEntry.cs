using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class UpdateTimeoffEntry : BaseCommand
    {
        public string EmployeeId { get; set; }
        public string TimeoffId { get; set; }
        public string TimeoffEntryId { get; set; }
        public int Type { get; set; }
        public double Hours { get; set; }
        public string? Label { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
