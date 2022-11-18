using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class AddTimesheetEntry : BaseCommand
    {
        public string EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public override CommandActionType ActionType() => CommandActionType.CREATION;
    }
}
