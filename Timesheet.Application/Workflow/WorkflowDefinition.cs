using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    public abstract class WorkflowDefinition
    {
        private readonly Dictionary<Enum, IList<Enum>> _permissionStatus = new();
        private readonly Dictionary<Enum, IList<EmployeeRoleOnData>> _authorizedRoles = new();

        public WorkflowDefinition(IEnumerable<Transition> transitions)
        {
            transitions.ToList().ForEach(transition =>
            {
                var superRoles = new EmployeeRoleOnData[] { EmployeeRoleOnData.ADMINISTRATOR };
                var otherRoles = transition.AuthorizedRoles ?? new EmployeeRoleOnData[] {};

                var transitionRoles = superRoles.Union(otherRoles).ToArray();

                AddTransiton(transition.Name, transition.PermissionStates);
                AddTransiton(transition.Name, transitionRoles);
            });
        }

        public void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole) where TEntity : Entity
        {
            if (!CanProcessTransition(transition, currentStatus, authorRole))
            {
                throw new InvalidWorkflowOperation<TEntity>(transition.ToString(), currentStatus.ToString(), entity.Id, authorRole.ToString());
            }
        }

        public bool CanProcessTransition(Enum transition, Enum currentStatus, EmployeeRoleOnData authorRole)
        {
            return EmployeeCanExecute(transition, authorRole) && CurrentStateAllowTransition(transition, currentStatus);
        }

        public virtual IEnumerable<Enum> NextTranstitions(Enum currentStatus, EmployeeRoleOnData authorRole)
        {
            var transitions = GetNextTransitions(currentStatus);
            var authorizedTransitions = new List<Enum>();

            foreach(var transition in transitions)
            {
                if (!_authorizedRoles.TryGetValue(transition, out var authorizedRoles))
                {
                    continue;
                }

                if(authorizedRoles.Any(role => role.Equals(authorRole)))
                {
                    authorizedTransitions.Add(transition);
                }
            }

            return authorizedTransitions;
        }

        private IEnumerable<Enum> GetNextTransitions(Enum currentStatus)
        {
            return _permissionStatus.Where(e => e.Value.Contains(currentStatus))
                .Select(e => e.Key)
                .ToList();
        }

        private bool EmployeeCanExecute(Enum transition, EmployeeRoleOnData authorRole)
        {
            if (!_authorizedRoles.TryGetValue(transition, out var authorizedRoles))
            {
                return false;
            }

            return authorizedRoles.Any(role => role.Equals(authorRole));
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