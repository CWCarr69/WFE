using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    internal class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRegistry _workflowRegistry;

        public WorkflowService(IWorkflowRegistry workflowRegistry)
        {
            this._workflowRegistry = workflowRegistry;
        }

        public bool CanProcessTransition(Type entityType, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole)
        {
            var workflow = _workflowRegistry.GetWorkflow(entityType);
            return workflow.CanProcessTransition(transition, currentStatus, authorRole);
        }

        public void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity
        {
            var workflow = _workflowRegistry.GetWorkflow(entity.GetType());
            workflow.AuthorizeTransition(entity, transition, currentStatus, authorRole);
        }

        public IEnumerable<Enum> NextTranstitions(Type entityType, Enum currentStatus, EmployeeRoleOnData authorRole)
        {
            var workflow = _workflowRegistry.GetWorkflow(entityType);
            return workflow.NextTranstitions(currentStatus, authorRole);
        }
    }
}
