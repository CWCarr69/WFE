
namespace Timesheet.Domain.Exceptions
{
    public sealed class EntityNotFoundException<TEntity> : DomainException where TEntity : Entity
    {
        public EntityNotFoundException(string Identifier) 
        : base($"{typeof(TEntity).Name}.NotFound", 404, $"{typeof(TEntity).Name} with identifier {Identifier} is not found.")
        {
        }
    }
}
