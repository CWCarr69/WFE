using Timesheet.Domain;

namespace Timesheet.FDPDataIntegrator.Services
{
    public interface IRepository<TEntity>
    {
        Task UpSert(TEntity entity);
        Task DisableConstraints();
        Task EnableConstraints();
        Task BeginTransaction(Action p);
        Task Delete(string id);
        IEnumerable<TEntity> GetRecents();
    }
}