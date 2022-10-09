using Timesheet.Domain;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.EmployeesIntegrator.Payrolls
{
    internal class PayrollAdapter
    {
        private IDictionary<string, bool> _employeePayrollIsWeekly;

        public PayrollAdapter(IDictionary<string, bool> employeePayrollIsHourly)
        {
            _employeePayrollIsWeekly = employeePayrollIsHourly;
        }

        public (TimesheetHeader, TimesheetEntry) Adapt (PayrollRecord record)
        {
            if(record == null || string.IsNullOrEmpty(record.RecordId) || string.IsNullOrEmpty(record.EmployeeCode))
            {
                throw new ArgumentNullException(nameof(record));
            }

            if(_employeePayrollIsWeekly.TryGetValue(record.EmployeeCode, out var isWeekly))
            {
                throw new ArgumentNullException($"Cannot find employee with code : {record.EmployeeCode}");
            }

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

            //Include EmployeeTimesheet if possible, and how about status
            (string payrollPeriod, DateTime start, DateTime end) timesheetInfos = isWeekly
                ? GetTimesheetWeeklyInfos(record.WorkDate)
                : GetTimesheetMonthlyInfos(record.WorkDate);

            var timesheetHeader = new TimesheetHeader(Entity.GenerateId(),
                payrollPeriod: timesheetInfos.payrollPeriod,
                startDate: timesheetInfos.start,
                endDate: timesheetInfos.end,
                status: TimesheetStatus.IN_PROGRESS //TODO update this to get the right status from FDP
            );

            return (timesheetHeader, timesheetEntry);
        }

        private (string payrollPeriod, DateTime start, DateTime end) GetTimesheetMonthlyInfos(DateTime workDate)
        {
            throw new NotImplementedException();
        }

        private (string payrollPeriod, DateTime start, DateTime end) GetTimesheetWeeklyInfos(DateTime workDate)
        {
            throw new NotImplementedException();
        }
    }
}
