namespace Timesheet.Domain.Exceptions
{
    public sealed class TimeoffInvalidDateIntervalException : DomainException
    {
        public TimeoffInvalidDateIntervalException(DateTime start, DateTime end) 
        : base("Employee.Timeoff.InvalidDates", 400, $"Timeoff start date {start} does'nt precede end date {end}.")
        {
        }
    }
}
