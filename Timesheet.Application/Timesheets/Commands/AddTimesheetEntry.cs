using Timesheet.Application.Shared;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Timesheets.Commands
{
    public class AddTimesheetEntry : BaseCommand
    {
        public string EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public string? ServiceOrderNumber { get; set; }
        public string? ServiceOrderDescription { get; set; }
        public string? JobNumber { get; set; }
        public string? JobDescription { get; set; }
        public string? JobTaskNumber { get; set; }
        public string? JobTaskDescription { get; set; }
        public string? LaborCode { get; set; }
        public string? CustomerNumber { get; set; }
        public string? ProfitCenterNumber { get; set; }
        public TimesheetEntryStatus? Status { get; set; }
        public bool OutOffCountry { get; set; }
        public override CommandActionType ActionType() => CommandActionType.CREATION;
    }
}
