namespace Timesheet.FDPDataIntegrator.Services
{
    internal interface IRepository<TEntity>
    {
        Task UpSert(TEntity entity);
        Task DisableConstraints();
        Task EnableConstraints();
        Task BeginTransaction(Action p);

    }
}