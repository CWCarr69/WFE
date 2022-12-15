using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class AddEntryToTimeoff : BaseCommand
    {
        public string? EmployeeId { get; set; }
        public string? TimeoffId { get; set; }
        public DateTime RequestDate { get; set; }
        public int Type { get; set; }
        public double Hours { get; set; }
        public string? Label { get; set; }

        public override CommandActionType ActionType() => CommandActionType.CREATION; 
    }
}
