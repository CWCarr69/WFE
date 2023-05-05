using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeWithTimeStatus
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimeoffId { get; set; }
        public string TimesheetId { get; set; }

        public double VacationSnapshot { private get; set; }
        public double PersonalSnapshot { private get; set; }
        public double VacationVariation { private get; set; }
        public double PersonalVariation { private get; set; }
        public bool ConsiderFixedBenefits { private get; set; }
        public double VacationBalance => ConsiderFixedBenefits ? VacationSnapshot - VacationVariation : VacationSnapshot;
        public double PersonalBalance => ConsiderFixedBenefits ? PersonalSnapshot - PersonalVariation : PersonalSnapshot;

        public int LastTimeoffStatus { get; set; }
        public string LastTimeoffEntryId { get; set; }
        public DateTime LastTimeoffRequestDate { get; set; }
        public string LastTimeoffStatusString => ((TimeoffStatus)LastTimeoffStatus).ToString();
        public bool IsLastTimeoffRequireApproval { get; set;  }
        public int LastTimesheetPartialStatus { get; set; }
        public int LastTimesheetStatus { get; set; }
        private bool IsFinalized => LastTimesheetStatus == (int)TimesheetStatus.FINALIZED;
        public string LastTimesheetPayrollPeriod { get; set; }
        public DateTime LastTimesheetWorkDate { get; set; }
        public string LastTimesheetStatusString 
            => IsFinalized 
                ? ((TimesheetStatus)LastTimesheetStatus).ToString()
                : ((TimesheetEntryStatus)LastTimesheetPartialStatus).ToString();

    }
}
