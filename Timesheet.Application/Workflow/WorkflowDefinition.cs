using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Exceptions;

namespace Timesheet.Application.Workflow
{
    public class WorkflowDefinition
    {
        private readonly Dictionary<Enum, IList<Enum>> _permissionStatus = new();
        private readonly Dictionary<Enum, IList<EmployeeRoleOnData>> _authorizedRoles = new();

        public WorkflowDefinition(IEnumerable<Transition> transitions)
        {
            transitions.ToList().ForEach(transition =>
            {
                AddTransiton(transition.Name, transition.PermissionStates);
                AddTransiton(transition.Name, transition.AuthorizedRoles);
            });
        }

        public void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity
        {
            if (!CanProcessTransition(entity, transition, currentStatus, authorRole))
            {
                throw new InvalidWorkflowOperation<TEntity>(transition.ToString(), currentStatus.ToString(), entity.Id);
            }
        }

        public bool CanProcessTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity
        {
            return EmployeeCanExecute(transition, authorRole) && CurrentStateAllowTransition(transition, currentStatus);
        }

        private bool EmployeeCanExecute(Enum transition, EmployeeRoleOnData authorRole)
        {
            if (!_authorizedRoles.TryGetValue(transition, out var authorizedRoles))
            {
                return false;
            }

            return _authorizedRoles.Any(role => role.Equals(authorRole));
        }

        private bool CurrentStateAllowTransition(Enum transition, Enum currentStatus)
        {
            if (!_permissionStatus.TryGetValue(transition, out var authorizedStatuses))
            {
                return false;
            }

            return authorizedStatuses.Any(
                status => status.Equals(currentStatus)
            );
        }

        public void AddTransiton(Enum transition, params Enum[] permissionStates)
        {
            _permissionStatus.Add(transition, permissionStates);
        }

        public void AddTransiton(Enum transition, params EmployeeRoleOnData[] authorizedRoles)
        {
            _authorizedRoles.Add(transition, authorizedRoles);
        }
    }
}