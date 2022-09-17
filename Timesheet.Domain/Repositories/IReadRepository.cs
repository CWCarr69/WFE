namespace Timesheet.Domain.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : Entity 
    {
        Task<TEntity?> Get(string identifier);
        IEnumerable<TEntity> Get();
    }
}
