namespace Timesheet.Domain.ReadModels.Employees
{
    public class HourInformation
    {
        public int Balance { get; set; }
        public int Used { get; set; }
        public int Scheduled { get; set; }
    }

    public class EmployeeBenefits
    {
        public int EligibleVacationHours { get; set; }
        public int EligiblePersonalHours { get; set; }
        public int RolloverHours { get; set; }

        public IEnumerable<HourInformation> Details { get; set; }
    }
}
