namespace Timesheet.Domain.ReadModels.Employees
{
    public class HourInformation
    {
        public string Type { get; set; }
        public double Balance { get; set; }
        public double Used { get; set; }
        public double Scheduled { get; set; }
    }

    public class EmployeeBenefits
    {
        public double EligibleVacationHours { get; set; }
        public double EligiblePersonalHours { get; set; }
        public double RolloverHours { get; set; }

        public IEnumerable<HourInformation> Details { get; set; }
    }
}
