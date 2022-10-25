namespace Timesheet.Domain.DomainEvents
{
    public record HolidayDeleted(string Id) : IDomainEvent;
}
