namespace Timesheet.FDPDataIntegrator.Services
{
    public interface IAdapter<TRecord, TEntity>
    {
        internal TEntity Adapt(TRecord record);
    }
}