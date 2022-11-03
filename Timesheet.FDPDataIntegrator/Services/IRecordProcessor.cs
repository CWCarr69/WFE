using Timesheet.Domain;

namespace Timesheet.FDPDataIntegrator.Services
{
    public interface IRecordProcessor<TAdapter, TRepository, TRecord, TEntity>
        where TEntity : Entity
        where TRepository : IRepository<TEntity>
        where TAdapter : IAdapter<TRecord, TEntity>
    {
        Task Process(TRecord[] records);
    }
}
