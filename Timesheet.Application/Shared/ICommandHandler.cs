namespace Timesheet.Application
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken token);

        bool CanExecute(TCommand command);
    }
}