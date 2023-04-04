using Microsoft.Extensions.Logging;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    public interface IPayrollRecordProcessor 
    : IRecordProcessor<
        IAdapter<PayrollRecord, TimesheetHeader>, 
        IRepository<TimesheetHeader>, 
        PayrollRecord, 
        TimesheetHeader
        >
    { }

    internal class PayrollRecordProcessor
    : RecordProcessor<IAdapter<PayrollRecord, TimesheetHeader>, IRepository<TimesheetHeader>, PayrollRecord, TimesheetHeader>, IPayrollRecordProcessor
    {
        private List<TimesheetEntry> _maybeDeletedTimesheetEntries;

        protected override List<TimesheetEntry> MayBeDeletedTimesheetEntries 
        {
            get
            {
                if(_maybeDeletedTimesheetEntries is null)
                {
                    _maybeDeletedTimesheetEntries = _repository
                        .GetRecents()
                        .Select(t => t.TimesheetEntries.First())
                        .ToList();
                }

                return _maybeDeletedTimesheetEntries;
            }   
        }

        public PayrollRecordProcessor(IRepository<TimesheetHeader> repository, IAdapter<PayrollRecord, TimesheetHeader> adapter, ILogger<PayrollRecordProcessor> logger)
            : base(repository, adapter, logger)
        {
        }

        public override bool SkipDeletion()
        {
            return false;
        }

        public override string RecordId(PayrollRecord record)
        {
            return record.RecordId;
        }
    }
}
