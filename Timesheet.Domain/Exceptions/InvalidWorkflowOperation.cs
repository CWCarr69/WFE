using Timesheet.Domain;
using Timesheet.Domain.Exceptions;

namespace Timesheet.Exceptions
{
    public class InvalidWorkflowOperation<TEntity> : DomainException where TEntity : Entity
    {
        public InvalidWorkflowOperation(string action, string status, string id, string role)
            : base($"Employee.{typeof(TEntity).Name}.InvalidTransition", 403, $"Role {role} is not allowed to {action} {typeof(TEntity)} (Id: {id}) with current status ({status}).")
        {
        }
    }
}
