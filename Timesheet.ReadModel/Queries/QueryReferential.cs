using System;
using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Models.Referential;
using PayrollType = Timesheet.Domain.ReadModels.Referential.PayrollType;

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
            var query = $@"SELECT distinct department as DepartmentName, department as DepartmentId
                FROM employees 
                WHERE department is not null
                ORDER BY department";
            var departments = await _dbService.QueryAsync<Department>(query);

            return departments;
        }

        public async Task<IEnumerable<PayrollPeriod>> GetPayrollPeriods()
        {
            var query = $@"SELECT distinct payrollPeriod as Code, StartDate, EndDate, Type
                            FROM timesheets order by payrollPeriod desc";
                            
            var payrollPeriods = await _dbService.QueryAsync<PayrollPeriod>(query);

            var now = DateTime.Now;
            var currentPeriodWeekly = payrollPeriods.FirstOrDefault(p => p.StartDate <= now && now <= p.EndDate && p.Type == TimesheetType.WEEKLY);
            var beforeCurrentPeriodWeekly = payrollPeriods.FirstOrDefault(p => p.EndDate <= currentPeriodWeekly?.EndDate && p.Type == TimesheetType.WEEKLY);

            var currentPeriodSalarly = payrollPeriods.FirstOrDefault(p => p.StartDate <= now && now <= p.EndDate && p.Type == TimesheetType.SALARLY);
            var beforeCurrentPeriodSalarly = payrollPeriods.FirstOrDefault(p => p.EndDate <= currentPeriodSalarly?.EndDate && p.Type == TimesheetType.SALARLY);

            if (beforeCurrentPeriodSalarly != null)
            {
                payrollPeriods.Remove(beforeCurrentPeriodSalarly);
                payrollPeriods.Insert(0, beforeCurrentPeriodSalarly);
            }

            if (beforeCurrentPeriodWeekly != null)
            {
                payrollPeriods.Remove(beforeCurrentPeriodWeekly);
                payrollPeriods.Insert(0, beforeCurrentPeriodWeekly);
            }

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

        public async Task<IEnumerable<PayrollType>> GetTimeoffTypes(bool requireApproval = true)
        {
            var categoryParam = "@category";
            var requireApprovalParam = "@requireApproval";
            var query = $"SELECT * FROM PayrollTypes WHERE Category = {categoryParam} AND requireApproval = {requireApprovalParam} ORDER BY PayrollCode";
            var timeoffTypes = await _dbService.QueryAsync<Domain.ReadModels.Referential.PayrollType>(query, new
            {
                category = PayrollTypesCategory.TIMEOFF,
                requireApproval
            });

            return timeoffTypes;
        }

        public async Task<IEnumerable<PayrollType>> GetAllTimeoffTypes()
        {
            var categoryParam = "@category";
            var query = $"SELECT * FROM PayrollTypes WHERE Category = {categoryParam} ORDER BY PayrollCode";
            var timeoffTypes = await _dbService.QueryAsync<Domain.ReadModels.Referential.PayrollType>(query, new
            {
                category = PayrollTypesCategory.TIMEOFF,
            });

            return timeoffTypes;
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

        public async Task<IEnumerable<PayrollType>> GetPayrollCodes()
        {
            var query = $"SELECT * FROM PayrollTypes ORDER BY PayrollCode";
            var payrollCodes = await _dbService.QueryAsync<PayrollType>(query);

            return payrollCodes;
        }
    }
}
