namespace Timesheet.Application
{
    public interface ISubCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public void SetParentCommandContext(IDictionary<string, object> context);
    }
}
