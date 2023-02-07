using Newtonsoft.Json;
using Timesheet.Domain;

namespace Timesheet.FDPDataIntegrator.Services
{
    internal abstract class RecordProcessor<TAdapter, TRepository, TRecord, TEntity> : IRecordProcessor<TAdapter, TRepository, TRecord, TEntity>
        where TEntity : Entity
        where TRepository : IRepository<TEntity>
        where TAdapter : IAdapter<TRecord, TEntity>
    {
        private readonly TRepository _repository;
        private readonly TAdapter _adapter;

        public RecordProcessor(TRepository repository, TAdapter adapter)
        {
            this._repository = repository;
            this._adapter = adapter;
        }

        public virtual async Task Process(TRecord[] records)
        {
            if(records is null)
            {
                await Task.CompletedTask;
            }

            foreach (var record in records)
            {
                try
                {
                    Console.WriteLine(JsonConvert.SerializeObject(record));
                    var entity = _adapter.Adapt(record);
                    if(entity != null)
                    {
                        await _repository.UpSert(entity);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
        }
    }
}
