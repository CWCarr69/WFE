using Timesheet.Domain.Models.Employees;
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
                new Transition(TimesheetEntryTransitions.SUBMIT, TimesheetEntryStatus.IN_PROGRESS, TimesheetEntryStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR, EmployeeRoleOnData.APPROVER),
                new Transition(TimesheetEntryTransitions.APPROVE, TimesheetEntryStatus.SUBMITTED)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
                new Transition(TimesheetEntryTransitions.REJECT, TimesheetEntryStatus.SUBMITTED, TimesheetEntryStatus.APPROVED)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
            })
        {
        }
    }
}
