using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Workflow
{
    internal class WorkflowRegistry : IWorkflowRegistry
    {
        private TimeoffWorkflow _timeoffWorkflow;
        private TimeoffEntryWorkflow _timeoffEntryWorkflow;
        private TimesheetWorkflow _timesheetWorkflow;
        private TimesheetEntryWorkflow _timesheetEntryWorkflow;

        public WorkflowRegistry()
        {
            _timeoffWorkflow = new TimeoffWorkflow();
            _timeoffEntryWorkflow = new TimeoffEntryWorkflow();
            _timesheetWorkflow = new TimesheetWorkflow();
            _timesheetEntryWorkflow = new TimesheetEntryWorkflow();

        }

        public WorkflowDefinition GetWorkflow(Type entityType)
        {
            if(entityType == null)
            {
                throw new NullReferenceException($"Null passed for entity {entityType}");
            }

            if (entityType == typeof(TimeoffHeader)) return _timeoffWorkflow;

            if (entityType == typeof(TimeoffEntry)) return _timeoffEntryWorkflow;

            if (entityType == typeof(TimesheetHeader)) return _timesheetWorkflow;

            if (entityType == typeof(TimesheetEntry)) return _timesheetEntryWorkflow;

            throw new InvalidOperationException($"No workflow is available for {entityType}");
        }
    }
}
