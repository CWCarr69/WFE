using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    internal interface IWorkflowService
    {
        bool CanProcessTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity;
        void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity;
    }
}
