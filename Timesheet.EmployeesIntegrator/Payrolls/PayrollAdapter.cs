using Timesheet.Domain;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal class PayrollAdapter : IAdapter<PayrollRecord, TimesheetHeader>
    {
        private IDictionary<string, bool> _employeePayrollIsWeekly;

        public PayrollAdapter(IDictionary<string, bool> employeePayrollIsHourly)
        {
            _employeePayrollIsWeekly = employeePayrollIsHourly;
        }

        public TimesheetHeader Adapt (PayrollRecord record)
        {
            if(record == null || string.IsNullOrEmpty(record.RecordId) || string.IsNullOrEmpty(record.EmployeeCode))
            {
                throw new ArgumentNullException(nameof(record));
            }

            if(_employeePayrollIsWeekly.TryGetValue(record.EmployeeCode, out var isWeekly))
            {
                throw new ArgumentNullException($"Cannot find employee with code : {record.EmployeeCode}");
            }

            //Include EmployeeTimesheet if possible otherwise fill the dictionary above first, and how about status
            (string payrollPeriod, DateTime start, DateTime end) timesheetInfos = isWeekly
                ? PayrollPeriodGenerator.GetTimesheetWeeklyInfos(record.WorkDate)
                : PayrollPeriodGenerator.GetTimesheetMonthlyInfos(record.WorkDate);

            var timesheetEntry = new TimesheetEntry(
                record.RecordId,
                employeeId: record.EmployeeCode,
                WorkDate: record.WorkDate,
                payrollCode: record.PayrollCode,
                hours: record.Quantity,
                description: record.CustomerName,
                serviceOrderNumber: record.ServiceOrderNumber,
                jobNumber: record.JobNumber,
                profitCenter: record.ProfitCenter
            );

            var timesheetHeader = new TimesheetHeader(Entity.GenerateId(),
                payrollPeriod: timesheetInfos.payrollPeriod,
                startDate: timesheetInfos.start,
                endDate: timesheetInfos.end,
                status: TimesheetStatus.IN_PROGRESS //TODO update this to get the right status from FDP
            );

            return timesheetHeader;
        }


    }
}
