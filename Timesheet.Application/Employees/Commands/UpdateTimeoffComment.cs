using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class UpdateTimeoffComment : BaseCommand
    {
       public string EmployeeId { get; set; }
       public string TimeoffId { get; set; }
       public string? EmployeeComment { get; set; }
       public string? ApproverComment { get; set; }
       public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
