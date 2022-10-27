using Timesheet.Application.Employees.Services;
using Timesheet.Domain;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseSubCommandHandler<TEntity, TCommand> : BaseCommandHandler<TEntity, TCommand>, ISubCommandHandler<TCommand>
        where TEntity : Entity
        where TCommand : ICommand
    {
        protected IDictionary<string, object> _parentCommandContext;

        protected bool LaunchedAsSubCommand => _parentCommandContext != null;

        public BaseSubCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
        }

        public override abstract Task<IEnumerable<IDomainEvent>> HandleCoreAsync(TCommand command, CancellationToken token);

        public void SetParentCommandContext(IDictionary<string, object> context)
        {
            _parentCommandContext = context;
        }
    }
}
