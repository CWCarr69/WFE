using Timesheet.Domain;

namespace Timesheet.Application.Workflow
{
    internal interface IWorkflowRegistry
    {
        WorkflowDefinition GetWorkflow<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
