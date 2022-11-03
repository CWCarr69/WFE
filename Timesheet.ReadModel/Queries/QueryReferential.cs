using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastruture.ReadModel.Queries
{
    public class QueryReferential : IQueryReferential
    {
        private readonly IDatabaseService _dbService;

        public QueryReferential(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<IEnumerable<Department>> GetDepartments()
        {
            var query = $"SELECT distinct department as DepartmentName FROM employees order by department";
            var departments = await _dbService.QueryAsync<Department>(query);

            return departments;
        }

        public async Task<IEnumerable<PayrollPeriod>> GetPayrollPeriods()
        {
            var query = $"SELECT distinct payrollPeriod as Code FROM timesheets order by payrollPeriod";
            var payrollPeriods = await _dbService.QueryAsync<PayrollPeriod>(query);

            return payrollPeriods;
        }

        public IEnumerable<EnumReadModel<TimeoffType>> GetTimeoffTypes()
        {
            return new List<EnumReadModel<TimeoffType>>
            {
                (EnumReadModel<TimeoffType>) TimeoffType.BERV,
                (EnumReadModel<TimeoffType>) TimeoffType.HOLIDAY,
                (EnumReadModel<TimeoffType>) TimeoffType.UNPAID,
                (EnumReadModel<TimeoffType>) TimeoffType.PERSONAL,
                (EnumReadModel<TimeoffType>) TimeoffType.VACATION,
                (EnumReadModel<TimeoffType>) TimeoffType.SHOP
            };
        }

        public IEnumerable<EnumReadModel<TimeoffStatus>> GetTimeoffStatuses()
        {
            return new List<EnumReadModel<TimeoffStatus>>
            {
                (EnumReadModel<TimeoffStatus>) TimeoffStatus.IN_PROGRESS,
                (EnumReadModel<TimeoffStatus>) TimeoffStatus.APPROVED,
                (EnumReadModel<TimeoffStatus>) TimeoffStatus.REJECTED,
                (EnumReadModel<TimeoffStatus>) TimeoffStatus.SUBMITTED,
            };
        }

        public IEnumerable<EnumReadModel<TimesheetStatus>> GetTimesheetStatuses()
        {
            return new List<EnumReadModel<TimesheetStatus>>
            {
                (EnumReadModel<TimesheetStatus>) TimesheetStatus.IN_PROGRESS,
                (EnumReadModel<TimesheetStatus>) TimesheetStatus.FINALIZED,
            };
        }

        public IEnumerable<EnumReadModel<TimesheetEntryStatus>> GetTimesheetEntryStatuses()
        {
            return new List<EnumReadModel<TimesheetEntryStatus>>
            {
                (EnumReadModel<TimesheetEntryStatus>) TimesheetEntryStatus.APPROVED,
                (EnumReadModel<TimesheetEntryStatus>) TimesheetEntryStatus.REJECTED,
                (EnumReadModel<TimesheetEntryStatus>) TimesheetEntryStatus.IN_PROGRESS,
                (EnumReadModel<TimesheetEntryStatus>) TimesheetEntryStatus.SUBMITTED
            };
        }
    }
}
