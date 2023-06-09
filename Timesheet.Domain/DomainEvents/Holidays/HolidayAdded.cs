namespace Timesheet.Domain.DomainEvents.Holidays
{
    public record HolidayAdded(string Id, DateTime Date, string Description) : IDomainEvent;
}
