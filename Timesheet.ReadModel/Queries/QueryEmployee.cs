using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastruture.ReadModel.Queries;
using Timesheet.Models.Referential;
using EmployeeBenefits = Timesheet.Domain.ReadModels.Employees.EmployeeBenefits;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public static class QueryEmployeeConstants
    {
        #region Employees
        private const string EmployeesQueryUsesTimesheetParam = "@usesTimesheet";
        public const string EmployeesQuery = $@"SELECT * FROM employees where usesTimesheet = {EmployeesQueryUsesTimesheetParam} order by Fullname";
        #endregion

        #region EmployeeProfile
        private const string EmployeeProfileQueryParam = "@id";
        private const string EmployeeProfileQueryEmailParam = "@email";
        private const string EmployeeProfileQueryLoginParam = "@userId";
        private const string WithManagerRoleBaseEmployeeProfileQuery = $@"SELECT DISTINCT e.*, 
            case when exists (select distinct 1 from employeeHierarchy h where h.managerid = e.id) then 1 else 0 end  as isManager 
            FROM employees e 
            ";

        public const string EmployeeProfileQuery = $@"{WithManagerRoleBaseEmployeeProfileQuery} WHERE id = {EmployeeProfileQueryParam}";
        public const string EmployeeProfileQueryByEmail = $@"{WithManagerRoleBaseEmployeeProfileQuery} WHERE email = {EmployeeProfileQueryEmailParam}";
        public const string EmployeeProfileQueryByLogin = $@"{WithManagerRoleBaseEmployeeProfileQuery} WHERE lower(userId) = lower({EmployeeProfileQueryLoginParam}) or replace(lower(userId), '@wilsonfire', '') = lower({EmployeeProfileQueryLoginParam})";
        #endregion

        #region EmployeeApprovers
        private const string EmployeeApproversParam = "@id";
        public const string EmployeeApproversQuery = $@"SELECT e.id as {nameof(EmployeeApprovers.EmployeeId)}, 
                e.PrimaryApproverId,
                p.FullName as {nameof(EmployeeApprovers.PrimaryApproverFullName)},   
                e.SecondaryApproverId,
                s.FullName as {nameof(EmployeeApprovers.SecondaryApproverFullName)}   
                FROM employees e 
                LEFT JOIN employees p on p.Id = e.PrimaryApproverId
                LEFT JOIN employees s on s.Id = e.SecondaryApproverId
                WHERE e.id = {EmployeeApproversParam}";

        #endregion

        #region EmployeeBenefits
        private const string EmployeeBenefitsParam = "@id";
        public const string EmployeeBenefitsQuery = $@"
            SELECT 
            CumulatedPreviousWorkPeriod as {nameof(EmployeeBenefits.CumulatedPreviousWorkPeriod)},
            ConsiderFixedBenefits as {nameof(EmployeeBenefits.ConsiderFixedBenefits)},
            VacationHours as  {nameof(EmployeeBenefits.VacationHours)},
            PersonalHours as {nameof(EmployeeBenefits.PersonalHours)},
            RolloverHours as {nameof(EmployeeBenefits.RolloverHours)}
            FROM employees e
            WHERE e.id = {EmployeeBenefitsParam}
        ";
        #endregion

        #region PendingTimeoffs
        private const string PendingTimeoffsQueryStatusParam = "@submittedStatus";

        private const string PendingTimeoffsQueryFromClause = $@"
                FROM employees e
                JOIN timeoffHeader t on e.Id = t.EmployeeId AND t.status = {PendingTimeoffsQueryStatusParam}
                JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
                JOIN payrollTypes pt on pt.numId = te.typeId
                JOIN timeoffHours thours on thours.id = t.id and thours.employeeId = e.id
        ";

        public const string TotalPendingTimeoffsQuery = $@"SELECT
            COUNT(DISTINCT CONCAT(e.Id, t.Id)) AS TotalItems
            {PendingTimeoffsQueryFromClause}
        ";

        public const string PendingTimeoffsQuery = $@"SELECT DISTINCT
            e.Id as {nameof(EmployeeTimeoff.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimeoff.FullName)},
            e.BenefitsSnapshot_VacationBalance as {nameof(EmployeeTimeoff.VacationSnapshot)}, 
            e.VacationHours + e.RolloverHours as {nameof(EmployeeTimeoff.VacationVariation)}, 
            e.BenefitsSnapshot_PersonalBalance as {nameof(EmployeeTimeoff.PersonalSnapshot)}, 
            e.PersonalHours as {nameof(EmployeeTimeoff.PersonalVariation)}, 
            t.Id as {nameof(EmployeeTimeoff.TimeoffId)},
            pt.PayrollCode AS {nameof(EmployeeTimeoff.PayrollCode)},
            t.CreatedDate  as {nameof(EmployeeTimeoff.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimeoff.ModifiedDate)},
            t.RequestStartDate  as {nameof(EmployeeTimeoff.RequestStartDate)},
            t.RequestEndDate  as {nameof(EmployeeTimeoff.RequestEndDate)},
            t.Status  as {nameof(EmployeeTimeoff.Status)},
            thours.totalHours as {nameof(EmployeeTimeoff.TotalHours)}
            {PendingTimeoffsQueryFromClause}
        ";

        public const string PendingTimeoffsQueryOrderByClause = $@"t.{ nameof(EmployeeTimeoff.RequestStartDate)}, e.{nameof(EmployeeTimesheet.FullName)}";

        #endregion

        #region PendingTimesheets
        private const string PendingTimesheetsQueryFinalizedStatusParam = "@timesheetFinalizedStatus";
        private const string PendingTimesheetsQueryEntryInProgressStatusParam = "@timesheetEntryInProgressStatus";
        private const string PendingTimesheetsQueryEntrySubmittedStatusParam = "@timesheetEntrySubmittedStatus";
        private const string PendingTimesheetsQueryEntryRejectedStatusParam = "@timesheetEntryRejectedStatus";
        private const string PendingTimesheetsQueryEntryApprovedStatusParam = "@timesheetEntryApprovedStatus";
        private const string PendingTimesheetsQueryPayrollCategoryParam = "@payrollCategory";

        #region PendingTimesheets Not Finalized
        private const string PendingTimesheetsQueryFromClause = $@"
            FROM timesheetHours
            WHERE PartialStatus = {PendingTimesheetsQueryEntrySubmittedStatusParam}
            AND Status != {PendingTimesheetsQueryFinalizedStatusParam}
        ";

        public const string TotalPendingTimesheetsQuery = $@"SELECT 
            COUNT(DISTINCT CONCAT(Id, EmployeeId)) AS totalItems
            {PendingTimesheetsQueryFromClause}
        ";

        public const string PendingTimesheetsQuery = $@"SELECT DISTINCT
            EmployeeId as {nameof(EmployeeTimesheet.EmployeeId)},
            Fullname as {nameof(EmployeeTimesheet.FullName)},
            Id as {nameof(EmployeeTimesheet.TimesheetId)},
            CreatedDate  as {nameof(EmployeeTimesheet.CreatedDate)},
            ModifiedDate  as {nameof(EmployeeTimesheet.ModifiedDate)},
            StartDate  as {nameof(EmployeeTimesheet.StartDate)},
            EndDate  as {nameof(EmployeeTimesheet.EndDate)},
            PayrollPeriod  as {nameof(EmployeeTimesheet.PayrollPeriod)},
            Status  as {nameof(EmployeeTimesheet.Status)},
            TotalHours as {nameof(EmployeeTimesheet.TotalHours)}
            {PendingTimesheetsQueryFromClause}
        ";

        public const string PendingTimesheetsQueryOrderByClause = $@"StartDate, Fullname";

        #endregion

        #region OrphanTimesheets
        private const string OrphanTimesheetsQueryFromClause = $@"
            FROM employees e
            JOIN timesheetEntry te on e.id = te.EmployeeId AND te.Status NOT IN ({PendingTimesheetsQueryEntryApprovedStatusParam}, {PendingTimesheetsQueryEntryRejectedStatusParam})
            JOIN PayrollTypes pt on pt.numId = te.PayrollCodeId and pt.category = {PendingTimesheetsQueryPayrollCategoryParam}
            JOIN timesheets t on t.Id = te.TimesheetHeaderId AND t.status = {PendingTimesheetsQueryFinalizedStatusParam}
        ";

        public const string TotalOrphanTimesheetsQuery = $@"SELECT 
            COUNT(DISTINCT CONCAT(t.Id, e.Id)) AS totalItems
            {OrphanTimesheetsQueryFromClause}
        ";

        public const string OrphanTimesheetsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimesheet.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimesheet.FullName)},
            t.Id as {nameof(EmployeeTimesheet.TimesheetId)},
            t.CreatedDate  as {nameof(EmployeeTimesheet.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimesheet.ModifiedDate)},
            t.StartDate  as {nameof(EmployeeTimesheet.StartDate)},
            t.EndDate  as {nameof(EmployeeTimesheet.EndDate)},
            t.PayrollPeriod  as {nameof(EmployeeTimesheet.PayrollPeriod)},
            max(t.Status)  as {nameof(EmployeeTimesheet.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimesheet.TotalHours)},
            SUM(CASE WHEN te.status = {PendingTimesheetsQueryEntrySubmittedStatusParam} THEN te.Hours ELSE 0 END) AS {nameof(EmployeeTimesheetWhithHoursPerStatus.TotalSubmitted)},
            SUM(CASE WHEN te.status = {PendingTimesheetsQueryEntryInProgressStatusParam} THEN te.Hours ELSE 0 END) AS {nameof(EmployeeTimesheetWhithHoursPerStatus.TotalInProgress)}
            {OrphanTimesheetsQueryFromClause}
        ";

        public const string OrphanTimesheetsQueryGroupByClause = $@"GROUP BY e.Id, e.Fullname, t.Id, t.CreatedDate, t.ModifiedDate, t.StartDate, t.EndDate, t.PayrollPeriod";
        public const string OrphanTimesheetsQueryOrderByClause = $@"t.{nameof(EmployeeTimesheet.StartDate)}, e.{nameof(EmployeeTimesheet.FullName)}";

        #endregion

        #endregion

        #region EmployeeTeam
        private const string EmployeeTeamQueryEmployeeIdParam = "@approverId";

        private const string EmployeeTeamQueryEmployeeTimesheetIsFinalized = $@"WITH employeeTimesheetIsFinalized 
            AS (
                SELECT e.id, CASE WHEN MIN(CAST(te.isFinalized AS INT)) IS NULL THEN 0 ELSE MIN(CAST(te.isFinalized as int)) END AS isFinalized
                FROM Employees e 
                LEFT JOIN timesheetEntry te ON te.EmployeeId = e.id
                GROUP BY e.id
            ),";

        private const string EmployeeTeamQueryLastTimesheetStatusPerEmployee = $@"LastTimesheets
            AS(
                SELECT EmployeeId, Status, PartialStatus, TimesheetHeaderId, PayrollPeriod, WorkDate
                FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER by Status ASC, PartialStatus ASC, StartDate ASC) AS rowNum, EmployeeId, Status, TimesheetHeaderId, PayrollPeriod, Workdate, PartialStatus
                    FROM FirstTimesheetEntryOfLastTimesheet f
                    JOIN employeeTimesheetIsFinalized e on e.id = f.EmployeeId and e.isFinalized = 0
                    UNION ALL
                    SELECT ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER by Status ASC, PartialStatus ASC, StartDate DESC) AS rowNum, EmployeeId, Status, TimesheetHeaderId, PayrollPeriod, Workdate, PartialStatus
                    FROM FirstTimesheetEntryOfLastTimesheet f
                    JOIN employeeTimesheetIsFinalized e on e.id = f.EmployeeId and e.isFinalized = 1
                ) T
                WHERE rowNum = 1
            ),
        ";

        private const string EmployeeTeamQueryLastTimeOffStatusPerEmployee = $@"LastTimeoffs
            AS(
                SELECT employeeId, status, timeoffHeaderId, requireApproval, requestDate
                FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY employeeId ORDER by status ASC, RequestStartDate DESC) AS rowNum, employeeId, status, TimeoffHeaderId, requireApproval, requestDate
                    FROM FirstTimeoffEntryOfLastTimeoff
                ) T
                WHERE rowNum = 1
            )
        ";

        private const string EmployeeTeamQueryUsesTimesheetParam = "@usesTimesheet";
        private const string EmployeeTeamQueryFromClause = $@"
            FROM employees e
            LEFT JOIN LastTimeoffs tos ON e.Id = tos.employeeId
            LEFT JOIN LastTimesheets ts ON e.Id = ts.EmployeeId
            WHERE e.Id = {EmployeeTeamQueryEmployeeIdParam} OR (
                (e.employmentDate is not null or (e.employmentDate is null and ts.TimesheetHeaderId is not null)) AND (e.usesTimesheet = {EmployeeTeamQueryUsesTimesheetParam})
                @clauseForDirectReport
            )
        ";

        public const string TotalEmployeeTeamQuery = $@"
            {EmployeeTeamQueryEmployeeTimesheetIsFinalized}
            {EmployeeTeamQueryLastTimesheetStatusPerEmployee}
            {EmployeeTeamQueryLastTimeOffStatusPerEmployee} 
            SELECT Count(DISTINCT e.Id) AS {nameof(EmployeeTeam.TotalItems)}
            {EmployeeTeamQueryFromClause}
        ";

        public const string EmployeeTeamQuery = $@"
            {EmployeeTeamQueryEmployeeTimesheetIsFinalized}
            {EmployeeTeamQueryLastTimesheetStatusPerEmployee}
            {EmployeeTeamQueryLastTimeOffStatusPerEmployee} 
            SELECT 
            e.Id as {nameof(EmployeeWithTimeStatus.EmployeeId)},
            e.FullName  as {nameof(EmployeeWithTimeStatus.FullName)}, 
            e.BenefitsSnapshot_VacationBalance as {nameof(EmployeeWithTimeStatus.VacationSnapshot)}, 
            e.VacationHours + e.RolloverHours as {nameof(EmployeeWithTimeStatus.VacationVariation)}, 
            e.BenefitsSnapshot_PersonalBalance as {nameof(EmployeeWithTimeStatus.PersonalSnapshot)}, 
            e.PersonalHours as {nameof(EmployeeWithTimeStatus.PersonalVariation)}, 
            tos.TimeoffHeaderId as {nameof(EmployeeWithTimeStatus.TimeoffId)},
            tos.Status as {nameof(EmployeeWithTimeStatus.LastTimeoffStatus)},
            --tos.timeoffEntryId as {nameof(EmployeeWithTimeStatus.LastTimeoffEntryId)},
            tos.requestDate as {nameof(EmployeeWithTimeStatus.LastTimeoffRequestDate)},
            tos.RequireApproval as  {nameof(EmployeeWithTimeStatus.IsLastTimeoffRequireApproval)},
            ts.TimesheetHeaderId as {nameof(EmployeeWithTimeStatus.TimesheetId)},
            ts.PartialStatus as {nameof(EmployeeWithTimeStatus.LastTimesheetPartialStatus)},
            ts.status as {nameof(EmployeeWithTimeStatus.LastTimesheetStatus)},
            ts.payrollPeriod as {nameof(EmployeeWithTimeStatus.LastTimesheetPayrollPeriod)},
            ts.workDate as {nameof(EmployeeWithTimeStatus.LastTimesheetWorkDate)}
            {EmployeeTeamQueryFromClause}
        ";

        public const string EmployeeTeamQueryOrderByClause = $@"e.{nameof(EmployeeWithTimeStatus.FullName)}";

        #endregion

        #region LightEmployeeTeam
        private const string LightEmployeeTeamQueryEmployeeIdParam = "@approverId";

        private const string LightEmployeeTeamQueryUsesTimesheetParam = "@usesTimesheet";
        private const string LightEmployeeTeamQueryFromClause = $@"
            FROM employees e
            WHERE e.Id = {LightEmployeeTeamQueryEmployeeIdParam} OR (
                (e.employmentDate is not null) AND (e.usesTimesheet = {LightEmployeeTeamQueryUsesTimesheetParam})
                @clauseForDirectReport
            )
            ORDER BY e.{nameof(EmployeeLight.FullName)}
        ";

        public const string LightEmployeeTeamQuery = $@"
            SELECT 
            e.Id as {nameof(EmployeeLight.EmployeeId)},
            e.FullName  as {nameof(EmployeeLight.FullName)}
            {LightEmployeeTeamQueryFromClause}
        ";
        #endregion

        #region UsedBenefits
        private const string CalculateUsedBenefitsQueryStartDateParam = "@start";
        private const string CalculateUsedBenefitsQueryEndDateParam = "@end";
        private const string CalculateUsedBenefitsQueryStatusParam = "@status";
        private const string CalculateUsedBenefitsQueryTypeParam = "@type";
        private const string CalculateUsedBenefitsQueryEmployeeIdParam = "@employeeId";

        public const string CalculateUsedBenefitsQuery = $@"SELECT SUM(hours)
            FROM Employees e
            JOIN TimeoffHeader th on e.Id = th.employeeId AND th.status = {CalculateUsedBenefitsQueryStatusParam}
            JOIN TimeoffEntry te on th.Id = te.TimeoffHeaderId
            WHERE e.Id = {CalculateUsedBenefitsQueryEmployeeIdParam}
            AND te.TypeId = {CalculateUsedBenefitsQueryTypeParam} 
            AND te.RequestDate 
            BETWEEN {CalculateUsedBenefitsQueryStartDateParam} AND {CalculateUsedBenefitsQueryEndDateParam}
        ";
        #endregion

        #region ScheduledBenefits
        private const string CalculateScheduledBenefitsQueryStartDateParam = "@start";
        private const string CalculateScheduledBenefitsQueryEndDateParam = "@end";
        private const string CalculateScheduledBenefitsQueryStatusParam = "@status";
        private const string CalculateScheduledBenefitsQueryTypeParam = "@type";
        private const string CalculateScheduledBenefitsQueryEmployeeIdParam = "@employeeId";

        public const string CalculateScheduledBenefitsQuery = $@"SELECT SUM(hours)
            FROM Employees e
            JOIN TimeoffHeader th on e.Id = th.employeeId AND th.status != {CalculateScheduledBenefitsQueryStatusParam}
            JOIN TimeoffEntry te on th.Id = te.TimeoffHeaderId
            WHERE e.Id = {CalculateScheduledBenefitsQueryEmployeeIdParam}
            AND te.TypeId = {CalculateScheduledBenefitsQueryTypeParam} 
            AND te.RequestDate 
            BETWEEN {CalculateScheduledBenefitsQueryStartDateParam} AND {CalculateScheduledBenefitsQueryEndDateParam}
        ";
        #endregion
    }

    public class QueryEmployee : BaseQuery, IQueryEmployee
    {
        private readonly IDatabaseService _dbService;

        public QueryEmployee(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<IEnumerable<EmployeeProfile?>> GetEmployees()
        {
            var query = QueryEmployeeConstants.EmployeesQuery;
            var employees = await _dbService.QueryAsync<EmployeeProfile>(query, new { UsesTimesheet = true});

            return employees;
        }

        public async Task<EmployeeProfile?> GetEmployeeProfile(string id, bool withApprovers = false)
        {
            var query = QueryEmployeeConstants.EmployeeProfileQuery;
            var employees = await _dbService.QueryAsync<EmployeeProfile>(query, new { id });

            var employee = employees.FirstOrDefault();

            if (employee is not null && withApprovers)
            {
                query = QueryEmployeeConstants.EmployeeApproversQuery;
                var employeeApprovers = await _dbService.QueryAsync<EmployeeApprovers>(query, new { id = id });
                employee.PrimaryApproverId = employeeApprovers.FirstOrDefault()?.PrimaryApproverId;
                employee.SecondaryApproverId = employeeApprovers.FirstOrDefault()?.SecondaryApproverId;

            }
            return employee;
        }

        public async Task<EmployeeProfile?> GetEmployeeProfileByEmail(string email)
        {
            var query = QueryEmployeeConstants.EmployeeProfileQueryByEmail;
            var employee = await _dbService.QueryAsync<EmployeeProfile>(query, new { email });

            return employee.FirstOrDefault();
        }

        public async Task<EmployeeProfile?> GetEmployeeProfileByLogin(string login)
        {
            var query = QueryEmployeeConstants.EmployeeProfileQueryByLogin;
            var employee = await _dbService.QueryAsync<EmployeeProfile>(query, new { userId=login });

            return employee.FirstOrDefault();
        }

        public async Task<EmployeeApprovers?> GetEmployeeApprovers(string id)
        {
            var query = QueryEmployeeConstants.EmployeeApproversQuery;
            var employee = await _dbService.QueryAsync<EmployeeApprovers>(query, new { id = id });

            return employee.FirstOrDefault();
        }

        public async Task<EmployeeBenefits?> GetEmployeeBenefitsVariation(string id)
        {
            var query = QueryEmployeeConstants.EmployeeBenefitsQuery;
            var employee = await _dbService.QueryAsync<EmployeeBenefits>(query, new { id = id });

            return employee.FirstOrDefault();
        }

        public async Task<EmployeeTeam> GetEmployeeTeam(int page, int itemsPerPage, string approverId = null, bool directReports = false)
        {
            var totalQuery = QueryEmployeeConstants.TotalEmployeeTeamQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery, whereKey: "", replace: "@clauseForDirectReport", addAnd: ADD_AND.AND_BEFORE);

            var query = QueryEmployeeConstants.EmployeeTeamQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query, whereKey: "", replace: "@clauseForDirectReport", addAnd: ADD_AND.AND_BEFORE);
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.EmployeeTeamQueryOrderByClause);

            var queryParams = new { approverId, usesTimesheet = true };
            var employeeTeam = await QueryWithTotal<EmployeeTeam, EmployeeWithTimeStatus>(queryParams, totalQuery, query);
                
            return employeeTeam;
        }

        public async Task<IEnumerable<EmployeeLight>> GetLightEmployeeTeam(string approverId = null, bool directReports = false)
        {
            var query = QueryEmployeeConstants.LightEmployeeTeamQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query, whereKey: "", replace: "@clauseForDirectReport", addAnd: ADD_AND.AND_BEFORE);

            var queryParams = new { approverId, usesTimesheet = true };
            var employeeTeam = await _dbService.QueryAsync<EmployeeLight>(query, queryParams);

            return employeeTeam;
        }

        public async Task<EmployeePendingTimeoffs> GetEmployeesPendingTimeoffs(int page, int itemsPerPage, string approverId = null, bool directReports = false)
        {
            var submittedStatus = TimeoffStatus.SUBMITTED;

            var totalQuery = QueryEmployeeConstants.TotalPendingTimeoffsQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery);

            var query = QueryEmployeeConstants.PendingTimeoffsQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query);
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.PendingTimeoffsQueryOrderByClause);

            var queryParams = new { submittedStatus, approverId };
            var employeePendingTimeoffs = await QueryWithTotal<EmployeePendingTimeoffs, EmployeeTimeoff>(queryParams, totalQuery, query);

            return employeePendingTimeoffs;
        }

        public async Task<EmployeePendingTimesheets> GetEmployeesPendingTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false)
        {

            var timesheetEntrySubmittedStatus = TimesheetEntryStatus.SUBMITTED;
            var timesheetFinalizedStatus = TimesheetStatus.FINALIZED;
            var payrollCategory = (int)PayrollTypesCategory.BILLABLE;

            var totalQuery = QueryEmployeeConstants.TotalPendingTimesheetsQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery, whereKey:"", employeeIdKey:"employeeId", addAnd:ADD_AND.AND_BEFORE);

            var query = QueryEmployeeConstants.PendingTimesheetsQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query, whereKey: "", employeeIdKey: "employeeId", addAnd: ADD_AND.AND_BEFORE);
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.PendingTimesheetsQueryOrderByClause);

            var queryParams = new { approverId, timesheetEntrySubmittedStatus, timesheetFinalizedStatus, payrollCategory };
            var employeePendingTimesheets = await QueryWithTotal<EmployeePendingTimesheets, EmployeeTimesheet>(queryParams, totalQuery, query);

            return employeePendingTimesheets;
        }

        public async Task<EmployeeOrphanTimesheets> GetEmployeesOrphanTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false)
        {
            var timesheetEntryInProgressStatus = TimesheetEntryStatus.IN_PROGRESS;
            var timesheetEntrySubmittedStatus = TimesheetEntryStatus.SUBMITTED;
            var timesheetEntryApprovedStatus = TimesheetEntryStatus.APPROVED;
            var timesheetEntryRejectedStatus = TimesheetEntryStatus.REJECTED;
            var timesheetFinalizedStatus = TimesheetStatus.FINALIZED;

            var payrollCategory = (int)PayrollTypesCategory.BILLABLE;


            var totalQuery = QueryEmployeeConstants.TotalOrphanTimesheetsQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery);

            var query = QueryEmployeeConstants.OrphanTimesheetsQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query);
            query = $"{query} {QueryEmployeeConstants.OrphanTimesheetsQueryGroupByClause}";
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.OrphanTimesheetsQueryOrderByClause);

            var queryParams = new { approverId, 
                timesheetEntryApprovedStatus, 
                timesheetEntryRejectedStatus, 
                timesheetFinalizedStatus, 
                payrollCategory,
                timesheetEntryInProgressStatus,
                timesheetEntrySubmittedStatus
            };

            var employeeOrphanTimesheets = await QueryWithTotal<EmployeeOrphanTimesheets, EmployeeTimesheetWhithHoursPerStatus>(queryParams, totalQuery, query);

            return employeeOrphanTimesheets;
        }

        public async Task<double> CalculateUsedBenefits(string employeeId, int type, DateTime start, DateTime end)
        {
            var query = QueryEmployeeConstants.CalculateUsedBenefitsQuery;

            var status = TimeoffStatus.APPROVED;
            return await _dbService.ExecuteScalarAsync<double>(query, new { start, end, status, type, employeeId });
        }

        public async Task<double> CalculateScheduledBenefits(string employeeId, int type)
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, 1, 1);
            var end = new DateTime(now.Year, 12, 31);

            var query = QueryEmployeeConstants.CalculateScheduledBenefitsQuery;

            var status = TimeoffStatus.APPROVED;
            return await _dbService.ExecuteScalarAsync<double>(query, new { start, end, status, type, employeeId });
        }

        private async Task<T> QueryWithTotal<T, U>(object queryParams, string totalQuery, string query)
            where T : WithTotal<U>
        {
            var withTotals = (await _dbService.QueryAsync<T>(totalQuery, queryParams)).FirstOrDefault();

            if (withTotals is not null)
            {
                var items = await _dbService.QueryAsync<U>(query, queryParams);
                withTotals.Items = items;
            }

            return withTotals;
        }

    }
}
