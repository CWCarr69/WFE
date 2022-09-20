using Timesheet.Application.Employees.Commands;

namespace Timesheet.Application
{
    public interface IDispatcher
    {
        void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent;
        void Publish<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : IDomainEvent;
        Task RunCommand<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand;
        Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> commandContext, CancellationToken token) where TCommand : ICommand;
    }
}
