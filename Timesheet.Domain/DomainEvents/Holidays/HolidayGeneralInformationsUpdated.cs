namespace Timesheet.Domain.DomainEvents.Holidays
{
    public record HolidayGeneralInformationsUpdated(string Id, string Description, string Notes) : IDomainEvent;
}
