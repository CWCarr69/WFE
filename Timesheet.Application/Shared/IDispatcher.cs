using Timesheet.Application.Employees.Commands;
using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface IDispatcher
    {
        void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent;
        void Publish<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : IDomainEvent;
        Task RunCommand<TCommand>(TCommand command, string authorId, CancellationToken token) where TCommand : ICommand;
        Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> commandContext, string authorId, CancellationToken token) where TCommand : ICommand;
    }
}
