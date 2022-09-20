using Timesheet.Domain;

namespace Timesheet.Application.Workflow
{
    internal interface IWorkflowService
    {
        bool CanProcessTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity;
        void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity;
    }
}
