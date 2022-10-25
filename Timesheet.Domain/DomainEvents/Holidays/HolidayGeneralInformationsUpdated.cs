namespace Timesheet.Domain.DomainEvents
{
    public record HolidayGeneralInformationsUpdated(string Id, string Description, string Notes) : IDomainEvent;
}
