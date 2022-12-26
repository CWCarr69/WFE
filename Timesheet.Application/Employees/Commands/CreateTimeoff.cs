using Timesheet.Application.Shared;
using Timesheet.Models.Referential;

namespace Timesheet.Application.Employees.Commands
{
    public class CreateTimeoff : BaseCommand
    {
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEndDate { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeComment { get; set; }

        public IEnumerable<AddEntryToTimeoff> Entries { get; set; }

        public bool RequireApproval 
        {
            get
            {
                if(Entries is null) return true;

                return !Entries.All(e => e.Type == (int)TimesheetFixedPayrollCodeEnum.SHOP && e.Type == (int)TimesheetFixedPayrollCodeEnum.UNPAID);
            }
        }

        public override CommandActionType ActionType() => CommandActionType.CREATION;

    }
}
