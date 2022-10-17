
using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Domain.Employees.Services
{
    public class EmployeeBenefitCalculator : IEmployeeBenefitCalculator
    {
        private const double WORK_DAY_HOURS = 8;
        IDictionary<(DateTime start, DateTime end), int> ScheduleFirstYears =
        new Dictionary<(DateTime start, DateTime end), int>()
        {
                        { (new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 2, 15)), 10 },
                        { (new DateTime(DateTime.Now.Year, 2, 16), new DateTime(DateTime.Now.Year, 3, 15)), 9 },
                        { (new DateTime(DateTime.Now.Year, 3, 16), new DateTime(DateTime.Now.Year, 4, 15)), 8 },
                        { (new DateTime(DateTime.Now.Year, 4, 16), new DateTime(DateTime.Now.Year, 5, 15)), 7 },
                        { (new DateTime(DateTime.Now.Year, 5, 16), new DateTime(DateTime.Now.Year, 6, 15)), 6 },
                        { (new DateTime(DateTime.Now.Year, 6, 16), new DateTime(DateTime.Now.Year, 8, 15)), 5 },
                        { (new DateTime(DateTime.Now.Year, 8, 16), new DateTime(DateTime.Now.Year, 9, 15)), 4 },
                        { (new DateTime(DateTime.Now.Year, 9, 16), new DateTime(DateTime.Now.Year, 10, 15)), 3 },
                        { (new DateTime(DateTime.Now.Year, 10, 16), new DateTime(DateTime.Now.Year, 11, 15)), 2 },
                        { (new DateTime(DateTime.Now.Year, 11, 16), new DateTime(DateTime.Now.Year, 12, 15)), 1 },
                        { (new DateTime(DateTime.Now.Year, 12, 16), new DateTime(DateTime.Now.Year, 12, 31)), 0 },
        };

        private IDictionary<(DateTime start, DateTime end), int> ScheduleOtherYears =
                new Dictionary<(DateTime start, DateTime end), int>()
                {
                    { (new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 2, 15)), 5 },
                    { (new DateTime(DateTime.Now.Year, 2, 16), new DateTime(DateTime.Now.Year, 3, 15)), 4 },
                    { (new DateTime(DateTime.Now.Year, 3, 16), new DateTime(DateTime.Now.Year, 4, 15)), 4 },
                    { (new DateTime(DateTime.Now.Year, 4, 16), new DateTime(DateTime.Now.Year, 5, 15)), 3 },
                    { (new DateTime(DateTime.Now.Year, 5, 16), new DateTime(DateTime.Now.Year, 6, 15)), 3 },
                    { (new DateTime(DateTime.Now.Year, 6, 16), new DateTime(DateTime.Now.Year, 8, 15)), 3 },
                    { (new DateTime(DateTime.Now.Year, 8, 16), new DateTime(DateTime.Now.Year, 9, 15)), 2 },
                    { (new DateTime(DateTime.Now.Year, 9, 16), new DateTime(DateTime.Now.Year, 10, 15)), 2 },
                    { (new DateTime(DateTime.Now.Year, 10, 16), new DateTime(DateTime.Now.Year, 11, 15)), 2 },
                    { (new DateTime(DateTime.Now.Year, 11, 16), new DateTime(DateTime.Now.Year, 12, 15)), 1 },
                    { (new DateTime(DateTime.Now.Year, 12, 16), new DateTime(DateTime.Now.Year, 12, 31)), 0 },
                };
        
        private readonly IQueryEmployee _queryEmployee;

        public EmployeeBenefitCalculator(IQueryEmployee queryEmployee)
        {
            this._queryEmployee = queryEmployee;
        }

        public async Task<EmployeeBenefits> GetBenefits(string employeeId, DateTime value)
        {
            var scheduledVacations = await GetScheduledVacationTimes(employeeId);
            var usedVacations = await GetUsedVacationTimes(employeeId);
            var totalVacations = GetTotalCurrentVacations(value);

            var scheduledPersonals = await GetScheduledPersonalTimes(employeeId);
            var usedPersonals = await GetUsedPersonalTimes(employeeId);
            var totalPersonals = GetTotalCurrentPersonalTimes();

            var personalHours = new HourInformation
            {
                Type = "Personal",
                Balance = totalPersonals - scheduledPersonals - usedPersonals,
                Used = usedPersonals,
                Scheduled = scheduledPersonals
            };

            var vacationHours = new HourInformation
            {
                Type = "Vacation",
                Balance = totalVacations - scheduledVacations - usedVacations,
                Used = usedVacations,
                Scheduled = scheduledVacations
            };

            var employeeBenefits = new EmployeeBenefits
            {
                EligibleVacationHours = totalVacations,
                EligiblePersonalHours = totalPersonals,
                RolloverHours = 0,
                Details = new List<HourInformation> { personalHours, vacationHours }
            };

            return employeeBenefits;
        }

        private Task<double> GetUsedPersonalTimes(string employeeId)
            => _queryEmployee
                .CalculateUsedBenefits(employeeId, Models.Employees.TimeoffType.PERSONAL, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));
        private Task<double> GetUsedVacationTimes(string employeeId)
            => _queryEmployee
                .CalculateUsedBenefits(employeeId, Models.Employees.TimeoffType.VACATION, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));

        private Task<double> GetScheduledPersonalTimes(string employeeId)
            => _queryEmployee
                .CalculateScheduledBenefits(employeeId, Models.Employees.TimeoffType.PERSONAL, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));
        private Task<double> GetScheduledVacationTimes(string employeeId)
            => _queryEmployee
                .CalculateScheduledBenefits(employeeId, Models.Employees.TimeoffType.VACATION, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));

        private double GetTotalCurrentPersonalTimes()
        {
            var months = DateTime.Now.Month;
            return months * WORK_DAY_HOURS;
        }

        private double GetTotalCurrentVacations(DateTime employmentDate)
        {
            var oneYearAfterEmploymentDate = employmentDate.Date.AddYears(1);
            if (DateTime.Now < oneYearAfterEmploymentDate)
            {
                return 0;
            }

            var vacations = 0;

            var anniversaryDate = new DateTime(DateTime.Now.Year, employmentDate.Month, employmentDate.Day);
            var yearsDifferencesSinceEmploymenDate = DateTime.Now.Year - employmentDate.Year;
            
            var isAnniversaryBenefitDate = PeriodIsSufficentForScheduledVacations(yearsDifferencesSinceEmploymenDate)
                && anniversaryDate <= DateTime.Now;

            if (isAnniversaryBenefitDate)
            {
                vacations += AtAnniversaryVacation(employmentDate, yearsDifferencesSinceEmploymenDate);
            }

            vacations += AdditionalVacations(yearsDifferencesSinceEmploymenDate);

            return vacations * WORK_DAY_HOURS;
        }

        private int AtAnniversaryVacation(DateTime employmentDate, int yearsSinceEmployment)
        {
            var schedule = yearsSinceEmployment == 1 ? ScheduleFirstYears : ScheduleOtherYears;

            foreach(var kvp in schedule)
            {
                if (kvp.Key.start <= employmentDate && employmentDate <= kvp.Key.end)
                {
                    return kvp.Value;
                }
            }

            return 0;
        }

        private int AdditionalVacations(int yearsConsidered)
        {
            if(yearsConsidered > 1) { return 10; }
            if(yearsConsidered > 5) { return 15; }
            if(yearsConsidered > 15) { return 20; }

            return 0;
        }

        private bool PeriodIsSufficentForScheduledVacations(int period)
        {
            return period == 1 || period == 5 || period == 15;
        }
    }
}
