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

        public double AdditionalVacationHours { get; set; }
        public double AdditionalPersonalHours { get; set; }
        public double AdditionalRolloverHours { get; set; }
        
        public IEnumerable<HourInformation> Details { get; set; }

        public string EligibleVacationHours => $@"{GetBalance(HourInformationType.Vacation)} / {TotalVacationHours + AdditionalVacationHours}{(AdditionalVacationHours == 0 ? "" : $"({TotalVacationHours} + {AdditionalVacationHours})" )}";
        public string EligiblePersonalHours => $@"{GetBalance(HourInformationType.Personal)} / {TotalPersonalHours + AdditionalPersonalHours}{(AdditionalPersonalHours == 0 ? "" : $"({TotalPersonalHours} + {AdditionalPersonalHours})")}";
        public string EligibleRolloverHours => $@"{RolloverHours + AdditionalRolloverHours}{(AdditionalRolloverHours == 0 ? "" : $"({RolloverHours} + {AdditionalRolloverHours})")}";

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
