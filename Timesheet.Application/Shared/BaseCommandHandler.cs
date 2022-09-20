using Timesheet.Domain;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IDispatcher _eventDispatcher;
        private readonly IUnitOfWork _transaction;

        private IEnumerable<IDomainEvent> _events = new List<IDomainEvent>();
        private IEnumerable<IDomainEvent> Events
        {
            get => _events;
            set
            {
                if (value != null || value.Any())
                {
                    _events = value;
                }
            }
        }

        public BaseCommandHandler(IDispatcher dispatcher, IUnitOfWork unitOfWork)
        {
            _eventDispatcher = dispatcher;
            _transaction = unitOfWork;
        }

        public abstract Task<IEnumerable<IDomainEvent>> HandleCore(TCommand command, CancellationToken token);

        public async Task HandleAsync(TCommand command, CancellationToken token)
        {
            if(command is null)
            {
                throw new Exception("Should set command before calling command Handler");
            }

            Events = await HandleCore(command, token);

            var completed = await _transaction.CompleteAsync(token);

            if (completed && this.Events.Any() && _eventDispatcher is not null)
            {
                this.PublishEvents();
            }

        }
        private void PublishEvents() => this._eventDispatcher.Publish(Events);

        public virtual bool CanExecute(TCommand command) => true;
    }
}
