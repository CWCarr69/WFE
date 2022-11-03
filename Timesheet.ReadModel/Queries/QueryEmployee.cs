using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastruture.ReadModel.Queries;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public static class QueryEmployeeConstants
    {
        #region Employees
        public const string EmployeesQuery = $@"SELECT * FROM employees";
        #endregion

        #region EmployeeProfile
        private const string EmployeeProfileQueryParam = "@id";
        private const string EmployeeProfileQueryEmailParam = "@email";
        private const string EmployeeProfileQueryLoginParam = "@login";
        private const string BaseEmployeeProfileQuery = $@"SELECT * FROM employees ";

        public const string EmployeeProfileQuery = $@"{BaseEmployeeProfileQuery} WHERE id = {EmployeeProfileQueryParam}";
        public const string EmployeeProfileQueryByEmail = $@"{BaseEmployeeProfileQuery} WHERE email = {EmployeeProfileQueryEmailParam}";
        public const string EmployeeProfileQueryByLogin = $@"{BaseEmployeeProfileQuery} WHERE login = {EmployeeProfileQueryLoginParam}";
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

        #region PendingTimeoffs
        private const string PendingTimeoffsQueryStatusParam = "@submittedStatus";

        private const string PendingTimeoffsQueryFromClause = $@"
                FROM employees e
                JOIN timeoffHeader t on e.Id = t.EmployeeId AND t.status = {PendingTimeoffsQueryStatusParam}
                JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
        ";

        public const string TotalPendingTimeoffsQuery = $@"SELECT
            COUNT(DISTINCT CONCAT(e.Id, t.Id)) AS TotalItems
            {PendingTimeoffsQueryFromClause}
        ";

        public const string PendingTimeoffsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimeoff.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimeoff.FullName)},
            t.Id as {nameof(EmployeeTimeoff.TimeoffId)},
            t.CreatedDate  as {nameof(EmployeeTimeoff.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimeoff.ModifiedDate)},
            t.RequestStartDate  as {nameof(EmployeeTimeoff.RequestStartDate)},
            t.RequestEndDate  as {nameof(EmployeeTimeoff.RequestEndDate)},
            t.Status  as {nameof(EmployeeTimeoff.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimeoff.TotalHours)}
            {PendingTimeoffsQueryFromClause}
        ";

        public const string PendingTimeoffsQueryGroupByClause = $@"GROUP BY e.Id, e.Fullname, t.Id, t.CreatedDate, t.ModifiedDate, t.RequestStartDate, t.RequestEndDate, t.status";
        public const string PendingTimeoffsQueryOrderByClause = $@"t.{ nameof(EmployeeTimeoff.CreatedDate)}";

        #endregion

        #region PendingTimesheets
        private const string PendingTimesheetsQueryStatusParam = "@timesheetFinalizedStatus";
        private const string PendingTimesheetsQueryEntryStatusParam = "@timesheetEntrySubmittedStatus";
        private const string PendingTimesheetsQueryRegularPayrollCodeParam = "@regularPayrollCode";
        private const string PendingTimesheetsQueryOvertimePayrollCodeParam = "@overtimePayrollCode";

        private const string PendingTimesheetsQueryFromClause = $@"
            FROM employees e
            JOIN timesheetEntry te on e.id = te.EmployeeId AND te.Status = {PendingTimesheetsQueryEntryStatusParam}
                AND te.PayrollCode  IN ({PendingTimesheetsQueryRegularPayrollCodeParam}, {PendingTimesheetsQueryOvertimePayrollCodeParam}) 
            JOIN timesheets t on t.Id = te.TimesheetHeaderId AND t.status != {PendingTimesheetsQueryStatusParam}
        ";

        public const string TotalPendingTimesheetsQuery = $@"SELECT 
            COUNT(DISTINCT CONCAT(t.Id, e.Id)) AS totalItems
            {PendingTimesheetsQueryFromClause}
        ";

        public const string PendingTimesheetsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimesheet.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimesheet.FullName)},
            t.Id as {nameof(EmployeeTimesheet.TimesheetId)},
            t.CreatedDate  as {nameof(EmployeeTimesheet.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimesheet.ModifiedDate)},
            t.StartDate  as {nameof(EmployeeTimesheet.StartDate)},
            t.EndDate  as {nameof(EmployeeTimesheet.EndDate)},
            t.PayrollPeriod  as {nameof(EmployeeTimesheet.PayrollPeriod)},
            max(t.Status)  as {nameof(EmployeeTimesheet.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimesheet.TotalHours)}
            {PendingTimesheetsQueryFromClause}
        ";

        public const string PendingTimesheetsQueryGroupByClause = $@"GROUP BY e.Id, e.Fullname, t.Id, t.CreatedDate, t.ModifiedDate, t.StartDate, t.EndDate, t.PayrollPeriod";
        public const string PendingTimesheetsQueryOrderByClause = $@"t.{ nameof(EmployeeTimesheet.CreatedDate)}";
        
        #endregion

        #region EmployeeTeam
        private const string EmployeeTeamQueryLastTimesheetStatusPerEmployee = $@"WITH LastTimesheets
            AS(
                SELECT employeeId, status, id
                FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY te.employeeId ORDER by t.CreatedDate DESC) AS rowNum, te.employeeId, t.status, t.id
                    FROM timesheets t
                    JOIN timesheetEntry te on t.id = te.TimesheetHeaderId
                ) T
                WHERE rowNum = 1
            ),
        ";

        private const string EmployeeTeamQueryLastTimeOffStatusPerEmployee = $@"LastTimeoffs
            AS(
                SELECT employeeId, status, id
                FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY employeeId ORDER by CreatedDate DESC) AS rowNum, employeeId, status, id
                    FROM timeoffHeader
                ) T
                WHERE rowNum = 1
            )
        ";

        private const string EmployeeTeamQueryFromClause = $@"
            FROM employees e
            LEFT JOIN LastTimeoffs tos ON e.Id = tos.employeeId
            LEFT JOIN LastTimesheets ts ON e.Id = ts.EmployeeId
        ";

        public const string TotalEmployeeTeamQuery = $@"
            {EmployeeTeamQueryLastTimesheetStatusPerEmployee}
            {EmployeeTeamQueryLastTimeOffStatusPerEmployee} 
            SELECT Count(DISTINCT e.Id) AS {nameof(EmployeeTeam.TotalItems)}
            {EmployeeTeamQueryFromClause}
        ";

        public const string EmployeeTeamQuery = $@"
            {EmployeeTeamQueryLastTimesheetStatusPerEmployee}
            {EmployeeTeamQueryLastTimeOffStatusPerEmployee} 
            SELECT 
            e.Id as {nameof(EmployeeWithTimeStatus.EmployeeId)},
            e.FullName  as {nameof(EmployeeWithTimeStatus.FullName)}, 
            tos.Id as {nameof(EmployeeWithTimeStatus.TimeoffId)},
            tos.Status as {nameof(EmployeeWithTimeStatus.LastTimeoffStatus)},
            ts.Id as {nameof(EmployeeWithTimeStatus.TimesheetId)},
            ts.Status as {nameof(EmployeeWithTimeStatus.LastTimesheetStatus)}
            {EmployeeTeamQueryFromClause}
        ";

        public const string EmployeeTeamQueryOrderByClause = $@"e.{nameof(EmployeeWithTimeStatus.FullName)}";

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
            AND te.Type = {CalculateUsedBenefitsQueryTypeParam} 
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
            AND te.Type = {CalculateScheduledBenefitsQueryTypeParam} 
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
            var employees = await _dbService.QueryAsync<EmployeeProfile>(query);

            return employees;
        }

        public async Task<EmployeeProfile?> GetEmployeeProfile(string id)
        {
            var query = QueryEmployeeConstants.EmployeeProfileQuery;
            var employee = await _dbService.QueryAsync<EmployeeProfile>(query, new { id });

            return employee.FirstOrDefault();
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
            var employee = await _dbService.QueryAsync<EmployeeProfile>(query, new { login });

            return employee.FirstOrDefault();
        }

        public async Task<EmployeeApprovers?> GetEmployeeApprovers(string id)
        {
            var query = QueryEmployeeConstants.EmployeeApproversQuery;
            var employee = await _dbService.QueryAsync<EmployeeApprovers>(query, new { id = id });

            return employee.FirstOrDefault();
        }

        public Task<EmployeeBenefits?> GetEmployeeBenefits(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<EmployeeTeam> GetEmployeeTeam(int page, int itemsPerPage, string approverId = null, bool directReports = false)
        {
            var totalQuery = QueryEmployeeConstants.TotalEmployeeTeamQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery);

            var query = QueryEmployeeConstants.EmployeeTeamQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query);
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.EmployeeTeamQueryOrderByClause);

            var queryParams = new { approverId };
            var employeeTeam = await QueryWithTotal<EmployeeTeam, EmployeeWithTimeStatus>(queryParams, totalQuery, query);
                
            return employeeTeam;
        }

        public async Task<EmployeePendingTimeoffs> GetEmployeesPendingTimeoffs(int page, int itemsPerPage, string approverId = null, bool directReports = false)
        {
            var submittedStatus = TimeoffStatus.SUBMITTED;

            var totalQuery = QueryEmployeeConstants.TotalPendingTimeoffsQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery);

            var query = QueryEmployeeConstants.PendingTimeoffsQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query);
            query = $"{query} {QueryEmployeeConstants.PendingTimeoffsQueryGroupByClause}";
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.PendingTimeoffsQueryOrderByClause);

            var queryParams = new { submittedStatus, approverId };
            var employeePendingTimeoffs = await QueryWithTotal<EmployeePendingTimeoffs, EmployeeTimeoff>(queryParams, totalQuery, query);

            return employeePendingTimeoffs;
        }

        public async Task<EmployeePendingTimesheets> GetEmployeesPendingTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false)
        {

            var timesheetEntrySubmittedStatus = TimesheetEntryStatus.SUBMITTED;
            var timesheetFinalizedStatus = TimesheetStatus.FINALIZED;
            var regularPayrollCode = TimesheetPayrollCode.REGULAR.ToString();
            var overtimePayrollCode = TimesheetPayrollCode.OVERTIME.ToString();

            var totalQuery = QueryEmployeeConstants.TotalPendingTimesheetsQuery;
            totalQuery = AddWhereClauseForDirectReports(approverId, directReports, totalQuery);

            var query = QueryEmployeeConstants.PendingTimesheetsQuery;
            query = AddWhereClauseForDirectReports(approverId, directReports, query);
            query = $"{query} {QueryEmployeeConstants.PendingTimesheetsQueryGroupByClause}";
            query = Paginate(page, itemsPerPage, query, QueryEmployeeConstants.PendingTimesheetsQueryOrderByClause);

            var queryParams = new { approverId, timesheetEntrySubmittedStatus, timesheetFinalizedStatus, regularPayrollCode, overtimePayrollCode };
            var employeePendingTimesheets = await QueryWithTotal<EmployeePendingTimesheets, EmployeeTimesheet>(queryParams, totalQuery, query);

            return employeePendingTimesheets;
        }

        public async Task<double> CalculateUsedBenefits(string employeeId, TimeoffType type, DateTime start, DateTime end)
        {
            var query = QueryEmployeeConstants.CalculateUsedBenefitsQuery;

            var status = TimeoffStatus.APPROVED;
            return await _dbService.ExecuteScalarAsync<double>(query, new { start, end, status, type, employeeId });
        }

        public async Task<double> CalculateScheduledBenefits(string employeeId, TimeoffType type)
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
