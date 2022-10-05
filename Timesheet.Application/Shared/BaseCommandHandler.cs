using Timesheet.Domain;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseCommandHandler<TEntity, TCommand> : ICommandHandler<TCommand>
        where TEntity : Entity
        where TCommand : ICommand
    {
        private readonly IAuditHandler _auditHandler;
        private readonly IDispatcher _eventDispatcher;
        private readonly IUnitOfWork _transaction;

        protected TEntity RelatedAuditableEntity { get; set; }

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

        public BaseCommandHandler(IAuditHandler auditHandler, IDispatcher dispatcher, IUnitOfWork unitOfWork)
        {
            _auditHandler = auditHandler;
            _eventDispatcher = dispatcher;
            _transaction = unitOfWork;
        }

        public abstract Task<IEnumerable<IDomainEvent>> HandleCore(TCommand command, CancellationToken token);

        public async Task HandleAsync(TCommand command, CancellationToken token)
        {
            string userId = "1";

            if(command is null)
            {
                throw new Exception("Should set command before calling command Handler");
            }

            Events = await HandleCore(command, token);

            if (RelatedAuditableEntity is not null)
            {
                _auditHandler.LogCommand(RelatedAuditableEntity, command, command.ActionType(), userId);
            }

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
