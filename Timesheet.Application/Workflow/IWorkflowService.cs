using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    public interface IWorkflowService
    {
        bool CanProcessTransition(Type entityType, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole);
        void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity;
        IEnumerable<Enum> NextTranstitions(Type entityType, Enum currentStatus, EmployeeRoleOnData authorRole);
    }
}
