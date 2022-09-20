using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseSubCommandHandler<TCommand> : BaseCommandHandler<TCommand>, ISubCommandHandler<TCommand> where TCommand : ICommand
    {
        protected IDictionary<string, object> _parentCommandContext;

        protected bool LaunchedAsSubCommand => _parentCommandContext != null;

        public BaseSubCommandHandler(IDispatcher dispatcher, IUnitOfWork unitOfWork) : base(dispatcher, unitOfWork)
        {
        }

        public override abstract Task<IEnumerable<IDomainEvent>> HandleCore(TCommand command, CancellationToken token);

        public void setParentCommandContext(IDictionary<string, object> context)
        {
            _parentCommandContext = context;
        }
    }
}
