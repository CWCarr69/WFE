using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    internal enum TimeoffEntryTransitions
    {
        DELETE, UPDATE
    }

    internal class TimeoffEntryWorkflow : WorkflowDefinition
    {

        public TimeoffEntryWorkflow()
            : base(new List<Transition>()
            {
                new Transition(TimeoffEntryTransitions.DELETE, TimeoffEntryStatus.NOT_PROCESSED, TimeoffEntryStatus.PROCESSED, TimeoffEntryStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR, EmployeeRoleOnData.APPROVER),
                new Transition(TimeoffEntryTransitions.UPDATE, TimeoffEntryStatus.NOT_PROCESSED, TimeoffEntryStatus.PROCESSED, TimeoffEntryStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR, EmployeeRoleOnData.APPROVER)
            })
        {
        }
    }
}
