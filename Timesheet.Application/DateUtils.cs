namespace Timesheet.Application.Timesheets
{
    internal static class DateUtils
    {
        private const int numberOfDaysPerWeek = 7;

        public static DateTime Previous(this DateTime referenceDate, DayOfWeek searchedDayOfWeek)
        {
            int diff;
            var referenceDayOfWeek = referenceDate.DayOfWeek;
            if (referenceDayOfWeek >= searchedDayOfWeek)
            {
                diff = referenceDayOfWeek - searchedDayOfWeek;
            }
            else
            {
                diff = numberOfDaysPerWeek - (searchedDayOfWeek - referenceDayOfWeek);
            }
            return referenceDate.AddDays(-diff);
        }

        private static DateTime Last(this DateTime referenceDate, DayOfWeek searchedDayOfWeek)
        {
            var diff = searchedDayOfWeek - referenceDate.DayOfWeek;
            return referenceDate.AddDays(diff);
        }

        public static DateTime Next(this DateTime now, DayOfWeek searchedDayOfWeek)
        {
            var diff = 0;
            if (now.DayOfWeek <= searchedDayOfWeek)
            {
                diff = DayOfWeek.Thursday - now.DayOfWeek;
            }
            else
            {
                diff = numberOfDaysPerWeek - (now.DayOfWeek - DayOfWeek.Thursday);
            }

            return now.AddDays(diff);
        }

        public static DateTime LastDayOfYear(this DateTime referenceDate) => new DateTime(referenceDate.Year, 12, 31);
        public static DateTime HalfDayOfMonth(this DateTime referenceDate) => new DateTime(referenceDate.Year, referenceDate.Month, 15);
        public static DateTime FirstDayOfMonth(this DateTime referenceDate) => new DateTime(referenceDate.Year, referenceDate.Month, 1);
        public static DateTime LastDayOfMonth(this DateTime referenceDate) => new DateTime(referenceDate.Year, referenceDate.Month, 1)
            .AddMonths(1)
            .AddDays(-1);

        public static DateTime SevenDaysBefore(this DateTime referenceDate) => referenceDate.AddDays(-numberOfDaysPerWeek + 1);

        public static DateTime LastDayOfYear(this DateTime referenceDate, DayOfWeek dateOfWeek)
        {
            var lastDayOfYear = referenceDate.LastDayOfYear();
            return lastDayOfYear.Last(dateOfWeek);
        }

        public static int WeeksBetweenDays(this DateTime referenceDate, DateTime secondReferenceDate)
        {
            decimal daysDiff = Math.Abs((referenceDate - secondReferenceDate).Days);
            var weeks = (int)(Math.Ceiling(daysDiff / numberOfDaysPerWeek));

            return weeks;
        }

        public static bool IsInSecondHalfOfMonth(this DateTime referenceDate) => referenceDate.Day > 15;
    }
}
