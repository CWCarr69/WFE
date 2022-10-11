using Timesheet.FDPDataIntegrator.Utils;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal class PayrollPeriodGenerator
    {
        internal static (string payrollPeriod, DateTime start, DateTime end) GetTimesheetMonthlyInfos(DateTime workDate)
        {
            var now = DateTime.Now;
            var isInSecondHalf = now.IsInSecondHalfOfMonth();
            var periodNumber = now.Month * 2 + (isInSecondHalf ? 2 : 1);

            var payrollPeriod = BiWeeklyPayrollPeriod(now, periodNumber);
            var start = isInSecondHalf ? now.HalfDayOfMonth().AddDays(1) : now.FirstDayOfMonth();
            var end = isInSecondHalf ? now.LastDayOfMonth() : now.HalfDayOfMonth();

            return (payrollPeriod, start, end);
        }

        internal static (string payrollPeriod, DateTime start, DateTime end) GetTimesheetWeeklyInfos(DateTime workDate)
        {
            var now = DateTime.Now;
            var oneYearBefore = DateTime.Now.AddYears(-1);
            var oneYearAfter = DateTime.Now.AddYears(1);

            var currentPayrollPeriodStartDate = oneYearBefore.LastDayOfYear(DayOfWeek.Thursday).SevenDaysBefore();
            var nextPayrollPeriodStartDate = now.LastDayOfYear(DayOfWeek.Thursday).SevenDaysBefore();

            var firstPayrollStartDate = now >= nextPayrollPeriodStartDate
                ? nextPayrollPeriodStartDate
                : currentPayrollPeriodStartDate;

            var nextThursday = now.Next(DayOfWeek.Thursday);
            var numberOfWeeks = nextThursday.WeeksBetweenDays(firstPayrollStartDate);

            var payrollPeriod = now >= nextPayrollPeriodStartDate
                ? WeeklyPayrollPeriod(oneYearAfter, numberOfWeeks)
                : WeeklyPayrollPeriod(now, numberOfWeeks);

            var start = now.Last(DayOfWeek.Friday);
            var end = nextThursday;

            return (payrollPeriod, start, end);
        }

        private static string WeeklyPayrollPeriod(DateTime date, int periodNumber) => $"{date.Year}-{periodNumber}H";

        private static string BiWeeklyPayrollPeriod(DateTime date, int periodNumber) => $"{date.Year}-{periodNumber}SS";
    }
}
