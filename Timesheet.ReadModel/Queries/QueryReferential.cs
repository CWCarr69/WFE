using System;
using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Referential;
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

        public async Task<IEnumerable<string>> GetTimeoffLabels()
        {
            var query = $"SELECT distinct label FROM timeoffentry order by label";
            var labels = await _dbService.QueryAsync<string>(query);

            return labels;
        }

        public async Task<IEnumerable<SimpleDictionaryItem>> GetJobs()
        {
            var query = $"SELECT distinct jobNumber AS [Key], jobDescription AS Value FROM timesheetentry WHERE jobNumber is not null order by jobNumber";
            var items = await _dbService.QueryAsync<SimpleDictionaryItem>(query);

            return items;
        }

        public async Task<IEnumerable<SimpleDictionaryItem>> GetJobTasks()
        {
            var query = $"SELECT distinct jobTaskNumber AS [Key], jobTaskDescription AS Value FROM timesheetentry WHERE jobTaskNumber is not null order by jobTaskNumber";
            var items = await _dbService.QueryAsync<SimpleDictionaryItem>(query);

            return items;
        }

        public async Task<IEnumerable<SimpleDictionaryItem>> GetServiceOrders()
        {
            var query = $"SELECT distinct ServiceOrderNumber AS [Key], ServiceOrderDescription AS Value FROM timesheetentry WHERE ServiceOrderNumber is not null order by ServiceOrderNumber";
            var items = await _dbService.QueryAsync<SimpleDictionaryItem>(query);

            return items;
        }

        public async Task<IEnumerable<string>> GetLaborCodes()
        {
            var query = $"SELECT distinct LaborCode FROM timesheetentry order by LaborCode";
            var laborCodes = await _dbService.QueryAsync<string>(query);

            return laborCodes;
        }

        public async Task<IEnumerable<string>> GetCustomerNumbers()
        {
            var query = $"SELECT distinct CustomerNumber FROM timesheetentry order by CustomerNumber";
            var customerNumbers = await _dbService.QueryAsync<string>(query);

            return customerNumbers;
        }

        public async Task<IEnumerable<string>> GetProfitCenters()
        {
            var query = $"SELECT distinct ProfitCenterNumber FROM timesheetentry order by ProfitCenterNumber";
            var profitCenterNumbers = await _dbService.QueryAsync<string>(query);

            return profitCenterNumbers;
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

        public IEnumerable<EnumReadModel<TimesheetPayrollCode>> GetPayrollCodes()
        {
            return new List<EnumReadModel<TimesheetPayrollCode>>
            {
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.REGULAR,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.OVERTIME,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.HOLIDAY,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.SHOP, 
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.BERV,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.JURY_DUTY,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.UNPAID,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.PERSONAL,
                (EnumReadModel<TimesheetPayrollCode>) TimesheetPayrollCode.VACATION,
            };
        }
    }
}
