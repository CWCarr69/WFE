using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Workflow
{
    internal enum TimesheetEntryTransitions
    {
        SUBMIT, APPROVE, REJECT
    }

    internal class TimesheetEntryWorkflow : WorkflowDefinition
    {

        public TimesheetEntryWorkflow()
            : base(new List<Transition>()
            {
                new Transition(TimesheetEntryTransitions.SUBMIT, TimesheetEntryStatus.IN_PROGRESS),
                new Transition(TimesheetEntryTransitions.APPROVE, TimesheetEntryStatus.SUBMITTED, TimesheetEntryStatus.REJECTED),
                new Transition(TimesheetEntryTransitions.REJECT, TimesheetEntryStatus.IN_PROGRESS, TimesheetEntryStatus.APPROVED)
            })
        {
        }
    }
}
