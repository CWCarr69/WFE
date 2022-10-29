using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Workflow
{
    public enum TimesheetTransitions
    {
        SUBMIT, APPROVE, REJECT,FINALIZE
    }

    internal class TimesheetWorkflow : WorkflowDefinition
    {

        public TimesheetWorkflow()
            : base(new List<Transition>()
            {
                new Transition(TimesheetTransitions.SUBMIT, TimesheetStatus.IN_PROGRESS)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimesheetTransitions.APPROVE, TimesheetStatus.IN_PROGRESS)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
                new Transition(TimesheetTransitions.REJECT, TimesheetStatus.IN_PROGRESS)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
                new Transition(TimesheetTransitions.FINALIZE, TimesheetStatus.IN_PROGRESS)
            })
        {
        }
    }
}
