namespace Timesheet.Domain.ReadModels.Employees
{
    public enum HourInformationType { Personal, Vacation }

    public class HourInformation
    {
        public string Type { get; set; }
        public double Balance { get; set; }
        public double Used { get; set; }
        public double Scheduled { get; set; }
    }

    public class EmployeeCalculatedBenefits
    {
        public double TotalVacationHours { get; set; }
        public double TotalPersonalHours { get; set; }
        public double RolloverHours { get; set; }
        public IEnumerable<HourInformation> Details { get; set; }

        public string EligibleVacationHours => $@"{GetBalance(HourInformationType.Vacation)} / {TotalVacationHours}";
        public string EligiblePersonalHours => $@"{GetBalance(HourInformationType.Personal)} / {TotalPersonalHours}";

        private double GetBalance(HourInformationType hourType)
        {
            return Details?.FirstOrDefault(d => d.Type == hourType.ToString())?.Balance ?? 0d;
        }

    }

    public class EmployeeBenefits
    {
        public double VacationHours { get; set; }
        public double PersonalHours { get; set; }
        public double RolloverHours { get; set; }
        public bool ConsiderFixedBenefits { get; set; }
        public int CumulatedPreviousWorkPeriod { get; set; }
    }
}
