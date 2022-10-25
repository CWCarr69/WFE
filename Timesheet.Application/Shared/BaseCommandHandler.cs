using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseCommandHandler<TEntity, TCommand> : ICommandHandler<TCommand>
        where TEntity : Entity
        where TCommand : ICommand
    {
        private readonly IEmployeeReadRepository _employeeReadRepository;
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

        public BaseCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler, 
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork)
        {
            this._employeeReadRepository = employeeReadRepository;
            _auditHandler = auditHandler;
            _eventDispatcher = dispatcher;
            _transaction = unitOfWork;
        }

        public abstract Task<IEnumerable<IDomainEvent>> HandleCoreAsync(TCommand command, CancellationToken token);

        public async Task HandleAsync(TCommand command, CancellationToken token)
        {
            if(command.AuthorId is null || !CanExecute(command))
            {
                throw new CannotExecuteCommandException(command.GetType().Name, command.AuthorId);
            }

            if(command is null)
            {
                throw new Exception("Should set command before calling command Handler");
            }

            Events = await HandleCoreAsync(command, token);

            if (RelatedAuditableEntity is not null)
            {
                _auditHandler.LogCommand(RelatedAuditableEntity, command, command.ActionType(), command.AuthorId);
            }

            if (this.Events.Any() && _eventDispatcher is not null)
            {
                //This call eventHandler. the eventHandlers are not supposed to complete the transaction
                await this.PublishEvents();
            }

            await _transaction.CompleteAsync(token);
        }

        private async Task PublishEvents() => await this._eventDispatcher.Publish(Events);

        public virtual bool CanExecute(TCommand command) => true;

        protected async Task<EmployeeRoleOnData> GetCurrentEmployeeRoleOnData(BaseCommand command, Employee? employee)
        {
            var author = command.AuthorId;
            var administrators = (await _employeeReadRepository.GetAdministrators())
                .Select(e => e.Id);

            var currentEmployeeRoleOnData = EmployeeRoleOnData.NONE;
            if (administrators.Contains(author))
            {
                currentEmployeeRoleOnData = EmployeeRoleOnData.ADMINISTRATOR;
            }
            else if (author is not null && author == employee?.Id)
            {
                currentEmployeeRoleOnData = EmployeeRoleOnData.CREATOR;
            }
            else if (author is not null && author == employee?.PrimaryApprover?.Id)
            {
                currentEmployeeRoleOnData = EmployeeRoleOnData.APPROVER;
            }
            else if (author is not null && author == employee?.SecondaryApprover?.Id)
            {
                currentEmployeeRoleOnData = EmployeeRoleOnData.APPROVER;
            }

            return currentEmployeeRoleOnData;
        }
    }
}
