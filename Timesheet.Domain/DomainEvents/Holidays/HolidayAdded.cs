namespace Timesheet.Domain.DomainEvents
{
    public record HolidayAdded(DateTime date, string description) : IDomainEvent;
}
