namespace Timesheet.FDPDataIntegrator.Services
{
    internal interface IAdapter<TRecord, TEntity>
    {
        internal TEntity Adapt(TRecord record);
    }
}