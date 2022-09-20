using Timesheet.Domain;

namespace Timesheet.Application.Workflow
{
    internal class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRegistry _workflowRegistry;

        public WorkflowService(IWorkflowRegistry workflowRegistry)
        {
            this._workflowRegistry = workflowRegistry;
        }

        public bool CanProcessTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity
        {
            var workflow = _workflowRegistry.GetWorkflow(entity);
            return workflow.CanProcessTransition(entity, transition, currentStatus);
        }

        public void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity
        {
            var workflow = _workflowRegistry.GetWorkflow(entity);
            workflow.AuthorizeTransition(entity, transition, currentStatus);
        }
    }
}
