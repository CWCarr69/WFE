using Timesheet.Application.Shared;
using Timesheet.Domain;

namespace Timesheet.Application
{
    public interface IAuditHandler
    {
        void LogCommand<TEntity, TCommand>(TEntity entity, TCommand command, CommandActionType auditType, string userId)
            where TEntity : Entity
            where TCommand : ICommand;
    }
}