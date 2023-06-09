namespace Timesheet.Domain.DomainEvents.Timesheets
{
    public record TimesheetFinalized(string payrollPeriod, DateTime startDate, DateTime endDate) : IDomainEvent;
}
