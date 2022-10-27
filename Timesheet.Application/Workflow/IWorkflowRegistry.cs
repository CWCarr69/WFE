using Timesheet.Domain;

namespace Timesheet.Application.Workflow
{
    public interface IWorkflowRegistry
    {
        WorkflowDefinition GetWorkflow(Type entityType);
    }
}
