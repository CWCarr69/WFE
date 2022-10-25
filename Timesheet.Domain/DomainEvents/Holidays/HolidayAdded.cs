namespace Timesheet.Domain.DomainEvents
{
    public record HolidayAdded(string Id, DateTime Date, string Description) : IDomainEvent;
}
