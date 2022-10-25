using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class DeleteTimeoffEntry : BaseCommand
    {
       public string EmployeeId { get; set; }
       public string TimeoffId { get; set; }
       public string TimeoffEntryId { get; set; }
        public override CommandActionType ActionType() => CommandActionType.DELETION;

    }
}
