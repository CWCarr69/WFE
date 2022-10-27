using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<IEnumerable<IDomainEvent>> HandleCoreAsync(TCommand command, CancellationToken token);
        Task HandleAsync(TCommand command, CancellationToken token);
    }
}