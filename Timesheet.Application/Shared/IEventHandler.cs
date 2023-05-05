using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface IEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task Handle(TDomainEvent @event, CancellationToken token);
    }
}