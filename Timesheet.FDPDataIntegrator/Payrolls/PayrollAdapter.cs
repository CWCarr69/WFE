using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.Models.Referential;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal class PayrollAdapter : IAdapter<PayrollRecord, TimesheetHeader>
    {
        private const string PAYROLL_HOURLY_TYPE = "HOURLY";
        private readonly List<PayrollType> _payrollTypes;

        public PayrollAdapter(IPayrollTypesRepository payrollTypesRepository) {
            _payrollTypes = payrollTypesRepository.GetPayrollTypes();
        }

        public TimesheetHeader Adapt (PayrollRecord record)
        {
            if(record == null || string.IsNullOrEmpty(record.RecordId) || string.IsNullOrEmpty(record.EmployeeCode))
            {
                throw new ArgumentNullException(nameof(record));
            }

            var isWeekly = record.PayrollType.ToUpper() == PAYROLL_HOURLY_TYPE;

            TimesheetHeader timesheetHeader = isWeekly
                ? TimesheetHeader.CreateWeeklyTimesheet(record.WorkDate)
                : TimesheetHeader.CreateMonthlyTimesheet(record.WorkDate);

            int payrollCodeId = _payrollTypes.SingleOrDefault(p => p.PayrollCode.ToUpper() == record.PayrollCode?.ToUpper())?.NumId ?? (int)TimesheetFixedPayrollCodeEnum.REGULAR;

            var timesheetEntry = new TimesheetEntry(
                record.RecordId,
                record.EmployeeCode,
                record.WorkDate,
                payrollCodeId,
                Math.Round(record.Quantity / 60, 2),
                record.CustomerName,
                record.ServiceOrderNumber,
                record.ServiceOrderDescription,
                jobNumber: record.JobNumber,
                jobDescription: record.JobDescription,
                jobTaskNumber: record.JobTaskNumber == "0" ? null : record.JobTaskNumber,
                profitCenter: record.ProfitCenter,
                false//TODO REMOVE (OUT OF COUNTRY)
            );

            timesheetHeader.UpdateMetadata(record.ModifyDate, record.ModifyDate);
            timesheetEntry.UpdateMetadata(record.ModifyDate, record.ModifyDate);
            timesheetHeader.AddTimesheetEntry(timesheetEntry);
            return timesheetHeader;
        }
    }
}
