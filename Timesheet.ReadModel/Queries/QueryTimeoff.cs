using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Workflow;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastruture.ReadModel.Queries;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    public static class QueryTimeoffConstants
    {
        #region TimeoffDetails
        private const string TimeoffDetailsQueryEmployeeIdParam = "@employeeId";
        private const string TimeoffDetailsQueryTimeoffIdParam = "@timeoffId";

        public const string TimeoffDetailsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimeoff.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimeoff.FullName)},
            t.Id as {nameof(EmployeeTimeoff.TimeoffId)},
            t.CreatedDate  as {nameof(EmployeeTimeoff.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimeoff.ModifiedDate)},
            t.RequestStartDate  as {nameof(EmployeeTimeoff.RequestStartDate)},
            t.RequestEndDate  as {nameof(EmployeeTimeoff.RequestEndDate)},
            t.EmployeeComment  as {nameof(EmployeeTimeoff.EmployeeComment)},
            t.ApproverComment  as {nameof(EmployeeTimeoff.ApproverComment)},
            t.Status  as {nameof(EmployeeTimeoff.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimeoff.TotalHours)}
            FROM employees e
            JOIN timeoffHeader t on e.Id = t.EmployeeId
            JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
            Where e.Id = {TimeoffDetailsQueryEmployeeIdParam} AND t.Id = {TimeoffDetailsQueryTimeoffIdParam}
            GROUP BY e.Id, e.Fullname, t.Id, t.CreatedDate, t.ModifiedDate, t.RequestStartDate, t.RequestEndDate, t.status, t.EmployeeComment, t.ApproverComment
        ";

        public const string TimeoffDetailsEntriesQuery = $@"SELECT
            Id as {nameof(EmployeeTimeoffEntry.Id)},
            CreatedDate AS {nameof(EmployeeTimeoffEntry.CreatedDate)},
            RequestDate AS {nameof(EmployeeTimeoffEntry.RequestDate)},
            Type AS {nameof(EmployeeTimeoffEntry.Type)},
            Hours AS {nameof(EmployeeTimeoffEntry.Hours)},
            Label AS {nameof(EmployeeTimeoffEntry.Label)}
            FROM timeoffEntry
            WHERE timeoffHeaderId = {TimeoffDetailsQueryTimeoffIdParam}
            ORDER BY RequestDate
        ";
        #endregion

        #region EmployeeTimeoffs
        private const string TimeoffsQueryEmployeeIdParam = "@employeeId";

        private const string TimeoffDetailsQueryFromClause = $@"
            FROM employees e
            JOIN timeoffHeader t on e.Id = t.EmployeeId
            JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
            Where e.Id = {TimeoffsQueryEmployeeIdParam}
            GROUP BY e.Id, e.Fullname, t.Id, t.CreatedDate, t.ModifiedDate, t.RequestStartDate, t.RequestEndDate, t.status
        ";

        public const string TimeoffDetailsQueryOrderByClause = $@"t.{nameof(EmployeeTimeoff.CreatedDate)} DESC";

        public const string TimeoffsTotalQuery = $@"SELECT
            COUNT(DISTINCT t.id) AS {nameof(EmployeeTimeoffHistory.TotalItems)}
            {TimeoffDetailsQueryFromClause}
        ";

        public const string TimeoffsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimeoff.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimeoff.FullName)},
            t.Id as {nameof(EmployeeTimeoff.TimeoffId)},
            t.CreatedDate  as {nameof(EmployeeTimeoff.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimeoff.ModifiedDate)},
            t.RequestStartDate  as {nameof(EmployeeTimeoff.RequestStartDate)},
            t.RequestEndDate  as {nameof(EmployeeTimeoff.RequestEndDate)},
            t.Status  as {nameof(EmployeeTimeoff.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimeoff.TotalHours)}
            {TimeoffDetailsQueryFromClause}
        ";
        #endregion

        #region EmployeeTimeoffsMonthStatistics
        private const string TimeoffsMonthStatisticsQueryEmployeeIdParam = "@employeeId";

        public const string TimeoffsMonthStatisticsQuery = $@"SELECT
            DATEADD(month, DATEDIFF(month, 0, te.RequestDate), 0) as {nameof(EmployeeTimeoffMonthStatistics.Month)},
            t.Status  as {nameof(EmployeeTimeoffMonthStatistics.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimeoffMonthStatistics.Hours)}
            FROM employees e
            JOIN timeoffHeader t on e.Id = t.EmployeeId
            JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
            Where e.Id = {TimeoffsMonthStatisticsQueryEmployeeIdParam}
            GROUP BY DATEADD(month, DATEDIFF(month, 0, te.RequestDate), 0), t.Status
        ";
        #endregion

        #region TimeoffSummary
        private const string TimeoffSummaryQueryEmployeeIdParam = "@employeeId";
        private const string TimeoffSummaryQueryTimeoffIdParam = "@timeoffId";

        public const string TimeoffSummaryQuery = $@"SELECT
            te.RequestDate as {nameof(EmployeeTimeoffDetailSummary.RequestDate)},
            te.Type  as {nameof(EmployeeTimeoffDetailSummary.Type)},
            SUM(te.Hours) as {nameof(EmployeeTimeoffDetailSummary.Hours)}
            FROM employees e
            JOIN timeoffHeader t on e.Id = t.EmployeeId
            JOIN timeoffEntry te on t.id = te.TimeoffHeaderId
            Where e.Id = {TimeoffSummaryQueryEmployeeIdParam} AND t.Id = {TimeoffSummaryQueryTimeoffIdParam}
            GROUP BY te.RequestDate, te.Type
        ";
        #endregion

        #region TimeoffEntriesInPeriod
        private const string TimeoffEntriesInPeriodEmployeeIdParam = "@employeeId";
        private const string TimeoffEntriesInPeriodStartParam = "@start";
        private const string TimeoffEntriesInPeriodEndParam = "@end";

        public const string TimeoffEntriesInPeriod = $@"SELECT
            th.Id as {nameof(EmployeeTimeoffEntry.TimeoffHeaderId)},
            te.Id as {nameof(EmployeeTimeoffEntry.Id)},
            te.RequestDate AS {nameof(EmployeeTimeoffEntry.RequestDate)},
            te.Type AS {nameof(EmployeeTimeoffEntry.Type)},
            te.Hours AS {nameof(EmployeeTimeoffEntry.Hours)}
            FROM timeoffEntry te 
            JOIN timeoffHeader th on th.Id = te.TimeoffHeaderId
            WHERE th.EmployeeId = {TimeoffEntriesInPeriodEmployeeIdParam}
            AND RequestDate BETWEEN {TimeoffEntriesInPeriodStartParam} AND {TimeoffEntriesInPeriodEndParam}
            ORDER BY RequestDate
        ";
        #endregion
    }

    public class QueryTimeoff : BaseQuery, IQueryTimeoff
    {
        private readonly IDatabaseService _dbService;

        public QueryTimeoff(IDatabaseService dbService, IWorkflowService workflowService)
        {
            this._dbService = dbService;
        }

        public async Task<EmployeeTimeoff> GetEmployeeTimeoffDetails(string employeeId, string timeoffId)
        {
            var query = QueryTimeoffConstants.TimeoffDetailsQuery;
            var timeoff = (await _dbService.QueryAsync<EmployeeTimeoff>(query, new { employeeId, timeoffId }))?.FirstOrDefault();

            if(timeoff is null)
            {
                return null;
            }

            query = QueryTimeoffConstants.TimeoffDetailsEntriesQuery;
            timeoff.Entries = await _dbService.QueryAsync<EmployeeTimeoffEntry>(query, new { timeoffId });

            return timeoff;
        }

        public async Task<EmployeeTimeoffHistory> GetEmployeeTimeoffs(string employeeId, int page, int itemsPerPage)
        {
            var totalQuery = QueryTimeoffConstants.TimeoffsTotalQuery;

            var query = QueryTimeoffConstants.TimeoffsQuery;
            query = Paginate(page, itemsPerPage, query, QueryTimeoffConstants.TimeoffDetailsQueryOrderByClause);

            var timeoffHistory = (await _dbService.QueryAsync<EmployeeTimeoffHistory>(totalQuery, new { employeeId }))?.FirstOrDefault();
            if(timeoffHistory is not null)
            {
                var timeoffs = await _dbService.QueryAsync<EmployeeTimeoff>(query, new { employeeId });
                timeoffHistory.Items = timeoffs;
            }

            return timeoffHistory;
        }

        public async Task<IEnumerable<EmployeeTimeoffMonthStatisticsGroupByMonth>> GetEmployeeTimeoffsMonthStatistics(string employeeId)
        {
            var query = QueryTimeoffConstants.TimeoffsMonthStatisticsQuery;
            var stats = await _dbService.QueryAsync<EmployeeTimeoffMonthStatistics>(query, new { employeeId });

            var statsPerMonth = stats.GroupBy(a => a.Month)
                .Select(s => new EmployeeTimeoffMonthStatisticsGroupByMonth
                {
                    Month = s.Key,
                    MonthStatistics = s
                });

            return statsPerMonth;
        }

        public async Task<IEnumerable<EmployeeTimeoffDetailSummary?>> GetEmployeeTimeoffSummary(string employeeId, string timeoffId)
        {
            var query = QueryTimeoffConstants.TimeoffSummaryQuery;
            var timeoffSummary = await _dbService.QueryAsync<EmployeeTimeoffDetailSummary>(query, new { employeeId, timeoffId });

            return timeoffSummary;
        }

        public async Task<IEnumerable<EmployeeTimeoffEntry>> GetEmployeeTimeoffEntriesInPeriod(string employeeId, DateTime start, DateTime end)
        {
            var query = QueryTimeoffConstants.TimeoffEntriesInPeriod;
            var entries = await _dbService.QueryAsync<EmployeeTimeoffEntry>(query, new { employeeId, start, end });

            return entries;
        }
    }
}