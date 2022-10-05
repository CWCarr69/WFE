using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class DeleteTimeoff : ICommand
    {
       public string EmployeeId { get; set; }
       public string TimeoffId { get; set; }
        public CommandActionType ActionType() => CommandActionType.DELETION;

    }
}
