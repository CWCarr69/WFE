namespace Timesheet.Domain.DomainEvents
{
    public record TimeoffEntryApproved(string Date, string type, double hours) : IDomainEvent;
}
