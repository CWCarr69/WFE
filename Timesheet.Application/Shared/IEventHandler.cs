using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface IEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent @event);
    }
}