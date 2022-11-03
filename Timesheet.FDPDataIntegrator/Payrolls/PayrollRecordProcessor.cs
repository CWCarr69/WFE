using Timesheet.Domain.Models.Employees;
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
        public PayrollRecordProcessor(IRepository<TimesheetHeader> repository, IAdapter<PayrollRecord, TimesheetHeader> adapter)
            : base(repository, adapter)
        {
        }
    }
}
