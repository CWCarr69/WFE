namespace Timesheet.Application
{
    public interface ISubCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public void setParentCommandContext(IDictionary<string, object> context);
    }
}
