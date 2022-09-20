namespace Timesheet.Domain.Exceptions
{
    internal class TimeOffEntryHoursExceededException : DomainException
    {
        public TimeOffEntryHoursExceededException(DateTime date, double maxmumHours)
            : base("Employee.Timeoff.Hours.Exceeded", 400, $"Timeoff on date {date.ToShortDateString} exceed maximum hours ({maxmumHours}).")
        {
        }
    }
}
