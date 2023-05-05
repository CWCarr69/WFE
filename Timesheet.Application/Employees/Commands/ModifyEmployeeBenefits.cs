using Timesheet.Application.Shared;

namespace Timesheet.Application.Employees.Commands
{
    public class ModifyEmployeeBenefits : BaseCommand
    {
        public string EmployeeId { get; set; }
        public double VacationHours { get; set; }
        public double PersonalHours { get; set; }
        public double RolloverHours { get; set; }
        public bool ConsiderFixedBenefits { get; set; }
        public int CumulatedPreviousWorkPeriod { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}
