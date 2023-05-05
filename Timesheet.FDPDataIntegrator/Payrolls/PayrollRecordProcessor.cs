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
        protected override List<TimesheetEntry> MayBeDeletedTimesheetEntries => _repository
            .GetRecents()
            .Select(t => t.TimesheetEntries.First())
            .ToList();

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
