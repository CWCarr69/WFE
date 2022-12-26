using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    internal enum TimeoffTransitions
    {
        SUBMIT, APPROVE, REJECT, DELETE, ADD_ENTRY, DELETE_ENTRY, UPDATE_ENTRY, UPDATE_COMMENT
    }

    internal class TimeoffWorkflow : WorkflowDefinition
    {

        public TimeoffWorkflow()
            : base(new List<Transition>()
            {
                new Transition(TimeoffTransitions.SUBMIT, TimeoffStatus.IN_PROGRESS, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimeoffTransitions.APPROVE, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
                new Transition(TimeoffTransitions.REJECT, TimeoffStatus.SUBMITTED)
                    .AuthorizeRoles(EmployeeRoleOnData.APPROVER),
                new Transition(TimeoffTransitions.DELETE, TimeoffStatus.IN_PROGRESS, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimeoffTransitions.ADD_ENTRY, TimeoffStatus.IN_PROGRESS, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimeoffTransitions.DELETE_ENTRY, TimeoffStatus.IN_PROGRESS, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimeoffTransitions.UPDATE_ENTRY, TimeoffStatus.IN_PROGRESS, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR),
                new Transition(TimeoffTransitions.UPDATE_COMMENT, TimeoffStatus.IN_PROGRESS, TimeoffStatus.SUBMITTED, TimeoffStatus.REJECTED)
                    .AuthorizeRoles(EmployeeRoleOnData.CREATOR, EmployeeRoleOnData.APPROVER),
            })
        {
        }
    }
}
