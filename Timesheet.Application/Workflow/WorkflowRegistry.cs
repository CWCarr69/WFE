using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    internal class WorkflowRegistry : IWorkflowRegistry
    {
        private TimeoffWorkflow _timeoffWorkflow;
        private TimeoffEntryWorkflow _timeoffEntryWorkflow;

        public WorkflowRegistry()
        {
            _timeoffWorkflow = new TimeoffWorkflow();
            _timeoffEntryWorkflow = new TimeoffEntryWorkflow();
        }

        public WorkflowDefinition GetWorkflow<TEntity>(TEntity entity) where TEntity : Entity
        {
            if(entity == null)
            {
                throw new NullReferenceException($"Null passed for entity {typeof(TEntity)}");
            }

            WorkflowDefinition workflow = entity switch
            {
                TimeoffHeader => _timeoffWorkflow,
                TimeoffEntry => _timeoffEntryWorkflow,
                _ => throw new InvalidOperationException($"No workflow is available for {typeof(TEntity)}")
            };

            return workflow!;
        }
    }
}
