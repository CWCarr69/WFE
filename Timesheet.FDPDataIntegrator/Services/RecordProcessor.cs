using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Newtonsoft.Json;
using Timesheet.Domain;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.FDPDataIntegrator.Services
{
    internal abstract class RecordProcessor<TAdapter, TRepository, TRecord, TEntity> : IRecordProcessor<TAdapter, TRepository, TRecord, TEntity>
        where TEntity : Entity
        where TRepository : IRepository<TEntity>
        where TAdapter : IAdapter<TRecord, TEntity>
    {
        protected readonly TRepository _repository;
        private readonly TAdapter _adapter;
        private readonly ILogger _logger;

        protected virtual List<TimesheetEntry> MayBeDeletedTimesheetEntries { get; } = new ();

        public RecordProcessor(TRepository repository, TAdapter adapter, ILogger logger)
        {
            this._repository = repository;
            this._adapter = adapter;
            this._logger = logger;
        }

        public virtual async Task Process(TRecord[] records)
        {
            if(records is null)
            {
                _logger.LogWarning("Null record found");
                await Task.CompletedTask;
            }

            var entries = new List<string>();

            foreach (var record in records)
            {
                try
                {
                    Console.WriteLine(JsonConvert.SerializeObject(record));
                    var entity = _adapter.Adapt(record);
                    if(entity != null)
                    {
                        if(!SkipDeletion())
                        {
                            entries.Add(RecordId(record));
                        }
                        await _repository.UpSert(entity);
                    }
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex.ToString());
                }
            }

            if (!SkipDeletion())
            {
                foreach (var entry in MayBeDeletedTimesheetEntries)
                {
                    _logger.LogInformation($"CHECKING DELETE OF {entry.Id}");
                    if(!entries.Any(e => e == entry.Id))
                    {
                        try
                        {
                            _logger.LogInformation($"TRY DELETE ENTRY {entry.Id}");
                            await _repository.Delete(entry.Id);
                        }catch(Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    }
                }
            }
        }

        public virtual bool SkipDeletion()
        {
            return true;
        }

        public virtual string RecordId(TRecord record)
        {
            return null;
        }
    }
}
