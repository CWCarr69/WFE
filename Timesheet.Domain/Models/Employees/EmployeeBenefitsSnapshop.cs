namespace Timesheet.Domain.Models.Employees
{
    public class EmployeeBenefitsSnapshop : ValueObject
    {
        public EmployeeBenefitsSnapshop(double vacationHours, double personalHours, double rolloverHours)
        {
            VacationHours = vacationHours;
            PersonalHours = personalHours;
            RolloverHours = rolloverHours;
        }

        public double VacationHours { get; private set; }
        public double PersonalHours { get; private set; }
        public double RolloverHours { get; private set; }

        public double VacationBalance { get; private set; }
        public double VacationUsed { get; private set; }
        public double VacationScheduled { get; private set; }

        public double PersonalBalance { get; private set; }
        public double PersonalUsed { get; private set; }
        public double PersonalScheduled { get; private set; }

        public EmployeeBenefitsSnapshop SetVacationDetails (double balance, double used, double scheduled)
        {
            VacationBalance = balance;
            VacationUsed = used;
            VacationScheduled = scheduled;

            return this;
        }

        public EmployeeBenefitsSnapshop SetPersonalDetails(double balance, double used, double scheduled)
        {
            PersonalBalance = balance;
            PersonalUsed = used;
            PersonalScheduled = scheduled;
            return this;
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return VacationHours;
            yield return PersonalHours;
            yield return RolloverHours;

            yield return VacationBalance;
            yield return VacationUsed;
            yield return VacationScheduled;

            yield return PersonalBalance;
            yield return PersonalUsed;
            yield return PersonalScheduled;
        }
    }
}
