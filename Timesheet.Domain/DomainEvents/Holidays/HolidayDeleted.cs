namespace Timesheet.Domain.DomainEvents.Holidays
{
    public record HolidayDeleted(string Id) : IDomainEvent;
}
