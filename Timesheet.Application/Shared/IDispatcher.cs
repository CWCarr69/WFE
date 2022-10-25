using Timesheet.Application.Employees.Commands;
using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface IDispatcher
    {
        Task Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent;
        Task Publish<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : IDomainEvent;
        Task RunCommand<TCommand>(TCommand command, string authorId, CancellationToken token) where TCommand : ICommand;
        Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> commandContext, string authorId, CancellationToken token) where TCommand : ICommand;
    }
}
