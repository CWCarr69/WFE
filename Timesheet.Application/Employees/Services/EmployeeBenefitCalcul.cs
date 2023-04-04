
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Models.Referential;

namespace Timesheet.Domain.Employees.Services
{
    public class EmployeeBenefitCalculator : IEmployeeBenefitCalculator
    {
        private const double WORK_DAY_HOURS = 8;
        private const double PERSONAL_TIME_UNITY = 4;
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

        public async Task<EmployeeCalculatedBenefits> GetBenefits(string employeeId, DateTime employmentDate)
        {
            var employeeProfile = await _queryEmployee.GetEmployeeProfile(employeeId);
            if(employeeProfile is null)
            {
                throw new EntityNotFoundException<Employee>(employeeId);
            }

            var employeeBenefitsVariations = await _queryEmployee.GetEmployeeBenefitsVariation(employeeId);

            var doNotUseCalculatedBenefits = !employeeProfile.ConsiderFixedBenefits;

            var scheduledVacations = await GetScheduledVacationTimes(employeeId);
            var usedVacations = await GetUsedVacationTimes(employeeId, DateTime.Now);
            var rollover = await GetTotalRollOverTimes(employeeId, employmentDate);
            var totalVacations = GetTotalCurrentVacationsTime(employmentDate, DateTime.Now);

            var scheduledPersonals = await GetScheduledPersonalTimes(employeeId);
            var usedPersonals = await GetUsedPersonalTimes(employeeId);
            var totalPersonals = GetTotalCurrentPersonalTimes();

            var personalHours = new HourInformation
            {
                Type = HourInformationType.Personal.ToString(),
                Balance = totalPersonals - scheduledPersonals - usedPersonals + (doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.PersonalHours),
                Used = usedPersonals,
                Scheduled = scheduledPersonals
            };

            var vacationHours = new HourInformation
            {
                Type = HourInformationType.Vacation.ToString(),
                Balance = totalVacations + rollover - scheduledVacations - usedVacations 
                + (doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.VacationHours)
                +(doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.RolloverHours),
                Used = usedVacations,
                Scheduled = scheduledVacations
            };

            var employeeCalcultatedBenefits = new EmployeeCalculatedBenefits
            {
                TotalVacationHours = totalVacations + rollover + (doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.VacationHours),
                TotalPersonalHours = totalPersonals + (doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.PersonalHours),
                RolloverHours = rollover + (doNotUseCalculatedBenefits ? 0 : employeeBenefitsVariations.RolloverHours),
                Details = new List<HourInformation> { personalHours, vacationHours }
            };

            return employeeCalcultatedBenefits;
        }

        private async Task<double> GetUsedPersonalTimes(string employeeId)
            => await _queryEmployee
                .CalculateUsedBenefits(employeeId, (int)TimesheetFixedPayrollCodeEnum.PERSONAL, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));

        private async Task<double> GetScheduledPersonalTimes(string employeeId)
            => await _queryEmployee.CalculateScheduledBenefits(employeeId, (int) TimesheetFixedPayrollCodeEnum.PERSONAL);

        private async Task<double> GetTotalRollOverTimes(string employeeId, DateTime employmentDate)
        {
            var now = DateTime.Now;
            var lastDayToTakeRollOverTimes = DateTime.Now.Date;
            if (now > lastDayToTakeRollOverTimes)
            {
                return 0;
            }

            var decembre31LastYear = new DateTime(now.Year - 1, 12, 31);
            var totalEligibleVaccationsLastYear = GetTotalCurrentVacationsTime(employmentDate, decembre31LastYear);
            var totalUsedVaccationsLastYear = await GetUsedVacationTimes(employeeId, decembre31LastYear);

            var rollover = totalEligibleVaccationsLastYear - totalUsedVaccationsLastYear;
            return rollover > 0 ? rollover : 0;
        }

        private double GetTotalCurrentPersonalTimes()
        {
            var months = DateTime.Now.Month - 1;
            return months * PERSONAL_TIME_UNITY;
        }

        private double GetTotalCurrentVacationsTime(DateTime employmentDate, DateTime now)
        {
            var oneYearAfterEmploymentDate = employmentDate.Date.AddYears(1);
            if (now < oneYearAfterEmploymentDate)
            {
                return 0;
            }

            var vacations = 0;

            var anniversaryDate = new DateTime(now.Year, employmentDate.Month, employmentDate.Day);
            var yearsDifferencesSinceEmploymenDate = now.Year - employmentDate.Year;
            
            var isAnniversaryBenefitDate = PeriodIsSufficentForBonusVacations(yearsDifferencesSinceEmploymenDate)
                && anniversaryDate <= now;

            if (isAnniversaryBenefitDate)
            {
                vacations += AtAnniversaryVacation(employmentDate, yearsDifferencesSinceEmploymenDate);
            }

            vacations += AdditionalVacations(yearsDifferencesSinceEmploymenDate);

            return vacations * WORK_DAY_HOURS;
        }

        private async Task<double> GetUsedVacationTimes(string employeeId, DateTime now)
            => await _queryEmployee
                .CalculateUsedBenefits(employeeId,
                (int) TimesheetFixedPayrollCodeEnum.VACATION, 
                new DateTime(now.Year, 1, 1), 
                new DateTime(now.Year, 12, 31));

        private async Task<double> GetScheduledVacationTimes(string employeeId)
            => await _queryEmployee.CalculateScheduledBenefits(employeeId, (int)TimesheetFixedPayrollCodeEnum.VACATION);

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

            if(yearsConsidered > 15) { return 20; }
            if (yearsConsidered > 5) { return 15; }
            if (yearsConsidered > 1) { return 10; }

            return 0;
        }

        private bool PeriodIsSufficentForBonusVacations(int period)
        {
            return period == 1 || period == 5 || period == 15;
        }
    }
}
