using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal class PayrollAdapter : IAdapter<PayrollRecord, TimesheetHeader>
    {
        private const string PAYROLL_HOURLY_TYPE = "HOURLY";

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

            string payrollCode = record.PayrollCode?.ToUpper();
            payrollCode = string.IsNullOrEmpty(payrollCode) ? TimesheetPayrollCode.OVERTIME.ToString() : payrollCode;

            var timesheetEntry = new TimesheetEntry(
                record.RecordId,
                employeeId: record.EmployeeCode,
                WorkDate: record.WorkDate,
                payrollCode: payrollCode,
                hours: record.Quantity,
                description: record.CustomerName,
                serviceOrderNumber: record.ServiceOrderNumber,
                serviceOrderDescription: record.ServiceOrderDescription,
                jobNumber: record.JobNumber,
                jobDescription: record.JobDescription,
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
