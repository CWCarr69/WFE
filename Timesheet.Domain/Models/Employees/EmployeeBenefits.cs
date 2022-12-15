namespace Timesheet.Domain.Models.Employees
{
    public class EmployeeBenefits : ValueObject
    {
        public EmployeeBenefits(double vacationHours, double personalHours, double rolloverHours)
        {
            VacationHours = vacationHours;
            PersonalHours = personalHours;
            RolloverHours = rolloverHours;
        }

        public double VacationHours { get; private set; }
        public double PersonalHours { get; private set; }
        public double RolloverHours { get; private set; }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return VacationHours;
            yield return PersonalHours;
            yield return RolloverHours;
        }
    }
}
