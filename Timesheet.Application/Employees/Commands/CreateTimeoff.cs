using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class CreateTimeoff : BaseCommand
    {
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEndDate { get; set; }
        public string? EmployeeId { get; set; }
        public string EmployeeComment { get; set; }

        public IEnumerable<AddEntryToTimeoff> Entries { get; set; }
        public override CommandActionType ActionType() => CommandActionType.CREATION;

    }
}
