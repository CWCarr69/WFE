using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application
{
    public interface IDispatcher
    {
        Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken token) where TDomainEvent : IDomainEvent;
        Task Publish<TDomainEvent>(IEnumerable<TDomainEvent> events, CancellationToken token) where TDomainEvent : IDomainEvent;
        Task RunCommand<TCommand>(TCommand command, User author, CancellationToken token) where TCommand : ICommand;
        Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> commandContext, User author, CancellationToken token) where TCommand : ICommand;
    }
}
