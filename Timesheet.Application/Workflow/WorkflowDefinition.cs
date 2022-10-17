using Timesheet.Domain;
using Timesheet.Exceptions;

namespace Timesheet.Application.Workflow
{
    public class WorkflowDefinition
    {
        private readonly Dictionary<Enum, IList<Enum>> _permissionStatus = new();

        public WorkflowDefinition(IEnumerable<Transition> transitions)
        {
            transitions.ToList().ForEach(transition =>
                AddTransiton(transition.Name, transition.PermissionStates));
        }

        public void AuthorizeTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity
        {
            if (!CanProcessTransition(entity, transition, currentStatus))
            {
                throw new InvalidWorkflowOperation<TEntity>(transition.ToString(), currentStatus.ToString(), entity.Id);
            }
        }

        public bool CanProcessTransition<TEntity>(TEntity entity, Enum transition, Enum currentStatus) where TEntity : Entity
        {
            if(!_permissionStatus.TryGetValue(transition, out var authorizedStatuses))
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
    }
}