using System;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Models.Referential;

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

            int payrollCodeId = string.IsNullOrEmpty(record.PayrollCode) ? (int)TimesheetFixedPayrollCodeEnum.OVERTIME : (int)TimesheetFixedPayrollCodeEnum.REGULAR;

            var timesheetEntry = new TimesheetEntry(
                record.RecordId,
                employeeId: record.EmployeeCode,
                WorkDate: record.WorkDate,
                payrollCodeId: payrollCodeId,
                hours: Math.Round(record.Quantity / 60, 2),
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
