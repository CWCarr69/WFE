using Timesheet.Domain;
using Timesheet.Domain.Exceptions;

namespace Timesheet.Exceptions
{
    public class InvalidWorkflowOperation<TEntity> : DomainException where TEntity : Entity
    {
        public InvalidWorkflowOperation(string action, string status, string id)
            : base($"Employee.{typeof(TEntity)}.InvalidDelete", 403, $"{action} is not allowed on {typeof(TEntity)} (Id: {id}) with it the current status ({status}).")
        {
        }
    }
}
