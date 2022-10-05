using Timesheet.Domain;

namespace Timesheet.Application.Workflow
{
    public interface IWorkflowRegistry
    {
        WorkflowDefinition GetWorkflow<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
