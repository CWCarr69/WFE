using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastruture.ReadModel.Queries;

namespace Timesheet.Infrastructure.Persistence.Queries
{

    public static class QueryTimesheetConstants
    {
        #region EmployeeTimesheetSummaryBy
        private const string TimesheetSummaryByPayrollCodeQueryEmployeeIdParam = "@employeeId";
        private const string TimesheetSummaryByPayrollCodeQueryTimesheetIdParam = "@timesheetId";

        public const string TimesheetSummaryByQuery = $@"
            SUM(te.Hours) as {nameof(EmployeeTimesheetDetailSummary.Hours)}
            FROM employees e
            JOIN timesheetEntry te on e.Id = te.EmployeeId
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            Where e.Id = {TimesheetSummaryByPayrollCodeQueryEmployeeIdParam} AND t.Id = {TimesheetSummaryByPayrollCodeQueryTimesheetIdParam}
        ";

        public const string TimesheetSummaryByDateQuery = $@"SELECT
            te.PayrollCode  as {nameof(EmployeeTimesheetDetailSummary.PayrollCode)},
            {TimesheetSummaryByQuery}
            GROUP BY te.PayrollCode
        ";

        public const string TimesheetSummaryByPayrollCodeQuery = $@"SELECT
            te.WorkDate  as {nameof(EmployeeTimesheetDetailSummary.WorkDate)},
            {TimesheetSummaryByQuery}
            GROUP BY te.WorkDate
        ";
        #endregion

        #region TimesheetDetails
        private const string TimesheetDetailsQueryEmployeeIdParam = "@employeeId";
        private const string TimesheetDetailsQueryTimesheetIdParam = "@timesheetId";

        public const string TimesheetDetailsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimesheet.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimesheet.FullName)},
            t.Id as {nameof(EmployeeTimesheet.TimesheetId)},
            t.PayrollPeriod as {nameof(EmployeeTimesheet.PayrollPeriod)},
            t.CreatedDate  as {nameof(EmployeeTimesheet.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimesheet.ModifiedDate)},
            t.StartDate  as {nameof(EmployeeTimesheet.StartDate)},
            t.EndDate  as {nameof(EmployeeTimesheet.EndDate)},
            t.Status  as {nameof(EmployeeTimesheet.Status)},
            c.EmployeeComment as {nameof(EmployeeTimesheet.EmployeeComment)},
            c.ApproverComment as {nameof(EmployeeTimesheet.ApproverComment)},
            SUM(te.Hours) as {nameof(EmployeeTimesheet.TotalHours)}
            FROM employees e
            JOIN timesheetEntry te on e.Id = te.EmployeeId
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            LEFT JOIN timesheetComment c ON c.TimesheetId = t.Id AND c.EmployeeId = e.Id
            Where e.Id = {TimesheetDetailsQueryEmployeeIdParam} AND t.Id = {TimesheetDetailsQueryTimesheetIdParam}
            GROUP BY e.Id, e.Fullname, t.Id, t.PayrollPeriod, t.CreatedDate, t.ModifiedDate,
            t.StartDate, t.EndDate, t.status, c.EmployeeComment, c.ApproverComment
        ";

        public const string TimesheetDetailsEntriesQuery = $@"SELECT
            Id AS {nameof(EmployeeTimesheetEntry.Id)},
            CreatedDate  as {nameof(EmployeeTimesheetEntry.CreatedDate)},
            ModifiedDate  as {nameof(EmployeeTimesheetEntry.ModifiedDate)},
            WorkDate  as {nameof(EmployeeTimesheetEntry.WorkDate)},
            PayrollCode  as {nameof(EmployeeTimesheetEntry.PayrollCode)},
            Hours  as {nameof(EmployeeTimesheetEntry.Quantity)},
            CustomerNumber  as {nameof(EmployeeTimesheetEntry.CustomerNumber)},
            JobNumber  as {nameof(EmployeeTimesheetEntry.JobNumber)},
            JobDescription  as {nameof(EmployeeTimesheetEntry.JobDescription)},
            LaborCode  as {nameof(EmployeeTimesheetEntry.LaborCode)},
            ServiceOrderNumber  as {nameof(EmployeeTimesheetEntry.ServiceOrderNumber)},
            ProfitCenterNumber  as {nameof(EmployeeTimesheetEntry.ProfitCenterNumber)},
            null  as {nameof(EmployeeTimesheetEntry.Department)},
            Description  as {nameof(EmployeeTimesheetEntry.Description)},
            OutOffCountry  as {nameof(EmployeeTimesheetEntry.OutOffCountry)},
            Status  as {nameof(EmployeeTimesheetEntry.Status)}
            FROM timesheetEntry
            WHERE employeeId = {TimesheetDetailsQueryEmployeeIdParam} AND timesheetHeaderId = {TimesheetDetailsQueryTimesheetIdParam}
        ";
        #endregion

        #region TimesheetReview
        private const string TimesheetReviewQueryIsSalariedParam = "@isSalaried";
        private const string TimesheetReviewQueryWeeklyTimesheetParam = "@weeklyTimesheet";
        private const string TimesheetReviewQueryMonthlyTimesheetParam = "@monthlyTimesheet";

        public const string TimesheetReviewQuerySearchFilterPlaceholder = "@searchFilter";
        public const string TimesheetReviewQueryPaginatePlaceholder = "@paginate";

        public const string TimesheetReviewTotalQuery = $@"SELECT
                SUM(tsr.Hours) AS {nameof(TimesheetReview.TotalQuantity)},
                COUNT(DISTINCT CONCAT(tsr.{nameof(TimesheetEntryDetails.EmployeeId)}, tsr.{nameof(TimesheetEntryDetails.TimesheetId)})) AS {nameof(TimesheetReview.TotalItems)}
                FROM 
                ( SELECT *
                    FROM 
                    (
                        SELECT e.id AS {nameof(TimesheetEntryDetails.EmployeeId)}, 
                        t.id AS {nameof(TimesheetEntryDetails.TimesheetId)}, 
                        te.Hours 
                        FROM employees e
                        JOIN timesheetEntry te on e.Id = te.EmployeeId
                        JOIN timesheets t on t.id = te.TimesheetHeaderId
                        WHERE 1=1 
                        { TimesheetReviewQuerySearchFilterPlaceholder }
                    ) t
                    UNION ALL(
                        SELECT e.id AS {nameof(TimesheetEntryDetails.EmployeeId)}, 
                        t.id AS {nameof(TimesheetEntryDetails.TimesheetId)}, 
                        tho.Hours 
                        FROM timesheetHoliday tho
                        JOIN timesheets t on t.id = tho.TimesheetHeaderId
                        JOIN employees e on (
                            (e.IsSalaried = {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryWeeklyTimesheetParam}) 
                            OR 
                            (e.IsSalaried != {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryMonthlyTimesheetParam}))
                        WHERE 1=1 
                        { TimesheetReviewQuerySearchFilterPlaceholder }
                    )  
                ) tsr
            ";

        public const string TimesheetReviewQueryEmployeeTimesheetEntries = $@"employeeTimesheetEntries AS
            (SELECT * FROM 
                (SELECT 
                    te.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                    te.EmployeeId AS EmployeeId,
                    te.TimesheetHeaderId AS TimesheetHeaderId,
                    te.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                    te.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                    te.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                    te.CustomerNumber  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                    te.JobNumber  as {nameof(TimesheetEntryDetails.JobNumber)},
                    te.JobDescription  as {nameof(TimesheetEntryDetails.JobDescription)},
                    te.LaborCode  as {nameof(TimesheetEntryDetails.LaborCode)},
                    te.ServiceOrderNumber  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                    te.ProfitCenterNumber  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                    e.Department  as {nameof(TimesheetEntryDetails.Department)},
                    te.Description  as {nameof(TimesheetEntryDetails.Description)},
                    te.OutOffCountry  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                    te.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
                    FROM timesheetEntry te
                    JOIN timesheets t on t.id = te.TimesheetHeaderId
                    JOIN employees e on e.Id = te.EmployeeId
                    WHERE 1=1 
                    { TimesheetReviewQuerySearchFilterPlaceholder }
                ) entries
                UNION ALL
                (SELECT 
                    tho.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                    e.Id as EmployeeId,
                    tho.TimesheetHeaderId AS TimesheetHeaderId,
                    tho.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                    tho.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                    tho.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                    null  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                    null  as {nameof(TimesheetEntryDetails.JobNumber)},
                    null  as {nameof(TimesheetEntryDetails.JobDescription)},
                    null  as {nameof(TimesheetEntryDetails.LaborCode)},
                    null  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                    null  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                    e.Department  as {nameof(TimesheetEntryDetails.Department)},
                    tho.Description  as {nameof(TimesheetEntryDetails.Description)},
                    null  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                    tho.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
                    FROM timesheetHoliday tho
                    JOIN timesheets t on t.id = tho.TimesheetHeaderId
                    JOIN employees e on (
                        (e.IsSalaried = {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryWeeklyTimesheetParam}) 
                        OR 
                        (e.IsSalaried != {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryMonthlyTimesheetParam}))
                    WHERE 1=1 
                    { TimesheetReviewQuerySearchFilterPlaceholder }
                )
            )";

        public const string TimesheetReviewQueryEmployeeTimesheetOrderByClause = $@"e.{nameof(TimesheetEntryDetails.Fullname)}";

        public const string TimesheetReviewQueryEmployeeTimesheet = $@"employeeTimesheet AS
            (SELECT
                e.Id AS {nameof(TimesheetEntryDetails.EmployeeId)},
                e.Fullname AS {nameof(TimesheetEntryDetails.Fullname)},
                t.Id AS {nameof(TimesheetEntryDetails.TimesheetId)},
                t.PayrollPeriod AS {nameof(TimesheetEntryDetails.PayrollPeriod)},
                SUM(CASE WHEN te.{nameof(TimesheetEntryDetails.PayrollCode)} = 'Overtime' 
                    THEN te.{nameof(TimesheetEntryDetails.Quantity)} else 0 END)
                    AS {nameof(TimesheetEntryDetails.Overtime)},
                SUM(te.{nameof(TimesheetEntryDetails.Quantity)}) AS {nameof(TimesheetEntryDetails.Total)},
                t.StartDate AS {nameof(TimesheetEntryDetails.StartDate)},
                t.EndDate AS {nameof(TimesheetEntryDetails.EndDate)},
                t.Status AS {nameof(TimesheetEntryDetails.Status)}
                FROM employees e
                JOIN employeeTimesheetEntries te on e.Id = te.EmployeeId
                JOIN timesheets t on t.id = te.TimesheetHeaderId
                WHERE 1=1 
                { TimesheetReviewQuerySearchFilterPlaceholder }
                GROUP BY e.Id, e.Fullname, t.Id, t.PayrollPeriod, t.StartDate, t.EndDate,  t.Status
                {TimesheetReviewQueryPaginatePlaceholder}
            )
        ";

        public const string TimesheetReviewQuery = $@"SELECT 
            et.{nameof(TimesheetEntryDetails.EmployeeId)},
            et.{nameof(TimesheetEntryDetails.Fullname)},
            et.{nameof(TimesheetEntryDetails.TimesheetId)},
            et.{nameof(TimesheetEntryDetails.PayrollPeriod)},
            et.{nameof(TimesheetEntryDetails.Overtime)},
            et.{nameof(TimesheetEntryDetails.Total)},
            et.{nameof(TimesheetEntryDetails.StartDate)},
            et.{nameof(TimesheetEntryDetails.EndDate)},
            et.{nameof(TimesheetEntryDetails.Status)},
            te.{nameof(TimesheetEntryDetails.TimesheetEntryId)},
            te.{nameof(TimesheetEntryDetails.WorkDate)},
            te.{nameof(TimesheetEntryDetails.PayrollCode)},
            te.{nameof(TimesheetEntryDetails.Quantity)},
            te.{nameof(TimesheetEntryDetails.CustomerNumber)},
            te.{nameof(TimesheetEntryDetails.JobNumber)},
            te.{nameof(TimesheetEntryDetails.JobDescription)},
            te.{nameof(TimesheetEntryDetails.LaborCode)},
            te.{nameof(TimesheetEntryDetails.ServiceOrderNumber)},
            te.{nameof(TimesheetEntryDetails.ProfitCenterNumber)},
            te.{nameof(TimesheetEntryDetails.Department)},
            te.{nameof(TimesheetEntryDetails.Description)},
            te.{nameof(TimesheetEntryDetails.OutOffCountry)},
            te.{nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
            FROM employeeTimesheet et
            JOIN employeeTimesheetEntries te on te.EmployeeId = et.employeeId AND te.TimesheetHeaderId = et.TimesheetId
        ";

        #endregion

        #region EmployeeTimesheets
        private const string EmployeeTimesheetsQueryEmployeeIdParam = "@employeeId";

        private const string EmployeeTimesheetsQueryFromClause = $@"FROM employees e
            JOIN timesheetEntry te on e.Id = te.EmployeeId
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            Where e.Id = {EmployeeTimesheetsQueryEmployeeIdParam}
            GROUP BY e.Id, e.Fullname, t.Id, t.PayrollPeriod, t.CreatedDate, t.ModifiedDate, t.StartDate, t.EndDate, t.status";

        public const string EmployeeTimesheetsQueryOrderByClause = $@"t.{nameof(EmployeeTimesheet.CreatedDate)} DESC";

        public const string EmployeeTimesheetsTotalQuery = $@"SELECT
            COUNT(DISTINCT t.Id) AS  {nameof(EmployeeTimesheetHistory.TotalItems)}
            {EmployeeTimesheetsQueryFromClause}
        ";

        public const string EmployeeTimesheetsQuery = $@"SELECT
            e.Id as {nameof(EmployeeTimesheet.EmployeeId)},
            e.Fullname as {nameof(EmployeeTimesheet.FullName)},
            t.Id as {nameof(EmployeeTimesheet.TimesheetId)},
            t.PayrollPeriod as {nameof(EmployeeTimesheet.PayrollPeriod)},
            t.CreatedDate  as {nameof(EmployeeTimesheet.CreatedDate)},
            t.ModifiedDate  as {nameof(EmployeeTimesheet.ModifiedDate)},
            t.StartDate  as {nameof(EmployeeTimesheet.StartDate)},
            t.EndDate  as {nameof(EmployeeTimesheet.EndDate)},
            t.Status  as {nameof(EmployeeTimesheet.Status)},
            SUM(te.Hours) as {nameof(EmployeeTimesheet.TotalHours)}
            {EmployeeTimesheetsQueryFromClause}
        ";
        #endregion

        #region Export
        private const string AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam = "@payrollPeriod";
        private const string AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam = "@isSalaried";
        private const string AllEmployeeTimesheetByPayrollPeriodQueryWeeklyTimesheetParam = "@weeklyTimesheet";
        private const string AllEmployeeTimesheetByPayrollPeriodQueryMonthlyTimesheetParam = "@monthlyTimesheet";

        public const string AllEmployeeTimesheetByPayrollPeriodQuery = $@"
            SELECT 
                t.payrollPeriod AS {nameof(AllEmployeesTimesheet<object>.PayrollPeriod)},
                t.status AS {nameof(AllEmployeesTimesheet<object>.Status)}
            FROM Timesheets t
            WHERE t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        #region External Export
        private const string ExternalAllEmployeeTimesheetEntriesByPayrollPeriodQuery = $@"
            SELECT 
                te.EmployeeId AS {nameof(ExternalTimesheetEntryDetails.No)},
                concat(replace(convert(varchar, te.WorkDate, 10),'-','/'), ' 1:01am') AS {nameof(ExternalTimesheetEntryDetails.Begin_Date)},
                concat(replace(convert(varchar, te.WorkDate, 10),'-','/'), ' 23:49:59') AS {nameof(ExternalTimesheetEntryDetails.End_Date)},
                CASE te.PayrollCode
                    WHEN '{nameof(TimesheetPayrollCode.REGULAR)}' THEN 'REG'
                    WHEN '{nameof(TimesheetPayrollCode.OVERTIME)}' THEN 'OT'
                    WHEN '{nameof(TimesheetPayrollCode.VACATION)}' THEN 'VAC'
                    WHEN '{nameof(TimesheetPayrollCode.SHOP)}' THEN 'SHOP'
                    WHEN '{nameof(TimesheetPayrollCode.BERV)}' THEN 'BERV'
                    WHEN '{nameof(TimesheetPayrollCode.UNPAID)}' THEN 'UNPAID'
                    WHEN '{nameof(TimesheetPayrollCode.JURY_DUTY)}' THEN 'EJURY'  
                END
                AS {nameof(ExternalTimesheetEntryDetails.DetCode)},
                te.Hours  as {nameof(ExternalTimesheetEntryDetails.Hours)},
                coalesce(te.JobNumber, te.ServiceOrderNumber) as {nameof(ExternalTimesheetEntryDetails.Job_Code)},
                te.ProfitCenterNumber  as {nameof(ExternalTimesheetEntryDetails.CC1)}
            FROM timesheetEntry te
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            JOIN employees e on e.Id = te.EmployeeId
            WHERE t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        private const string ExternalAllEmployeeTimesheetHolidaysByPayrollPeriodQuery = $@"
            SELECT 
                e.Id AS {nameof(ExternalTimesheetEntryDetails.No)},
                concat(replace(convert(varchar, tho.WorkDate, 10),'-','/'), ' 1:01am') AS {nameof(ExternalTimesheetEntryDetails.Begin_Date)},
                concat(replace(convert(varchar, tho.WorkDate, 10),'-','/'), ' 23:49:59') AS {nameof(ExternalTimesheetEntryDetails.End_Date)},
                CASE tho.PayrollCode
                    WHEN '{nameof(TimesheetPayrollCode.REGULAR)}' THEN 'REG'
                    WHEN '{nameof(TimesheetPayrollCode.OVERTIME)}' THEN 'OT'
                    WHEN '{nameof(TimesheetPayrollCode.VACATION)}' THEN 'VAC'
                    WHEN '{nameof(TimesheetPayrollCode.SHOP)}' THEN 'SHOP'
                    WHEN '{nameof(TimesheetPayrollCode.BERV)}' THEN 'BERV'
                    WHEN '{nameof(TimesheetPayrollCode.UNPAID)}' THEN 'UNPAID'
                    WHEN '{nameof(TimesheetPayrollCode.JURY_DUTY)}' THEN 'EJURY'                   
                END
                AS {nameof(ExternalTimesheetEntryDetails.DetCode)},
                tho.Hours  as {nameof(ExternalTimesheetEntryDetails.Hours)},
                null AS {nameof(ExternalTimesheetEntryDetails.Job_Code)},
                null  as {nameof(ExternalTimesheetEntryDetails.CC1)}
            FROM timesheetHoliday tho
            JOIN timesheets t on t.id = tho.TimesheetHeaderId
            JOIN employees e on (
                (e.IsSalaried = {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryWeeklyTimesheetParam}) 
                OR 
                (e.IsSalaried != {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryMonthlyTimesheetParam}))
            WHERE t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        public const string ExternalAllEmployeeTimesheetAllEntriesByPayrollPeriodQuery = $@"
            SELECT * 
            FROM ({ExternalAllEmployeeTimesheetEntriesByPayrollPeriodQuery}) entries
            UNION ALL ({ExternalAllEmployeeTimesheetHolidaysByPayrollPeriodQuery})
            ORDER BY 2
        ";
        #endregion

        #region Export web
        private const string AllEmployeeTimesheetEntriesByPayrollPeriodQuery = $@"
            SELECT 
                te.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                te.EmployeeId AS EmployeeId,
                te.TimesheetHeaderId AS TimesheetHeaderId,
                te.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                te.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                te.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                te.CustomerNumber  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                te.JobNumber  as {nameof(TimesheetEntryDetails.JobNumber)},
                te.JobDescription  as {nameof(TimesheetEntryDetails.JobDescription)},
                te.LaborCode  as {nameof(TimesheetEntryDetails.LaborCode)},
                te.ServiceOrderNumber  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                te.ProfitCenterNumber  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                e.Department  as {nameof(TimesheetEntryDetails.Department)},
                te.Description  as {nameof(TimesheetEntryDetails.Description)},
                te.OutOffCountry  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                te.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
            FROM timesheetEntry te
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            JOIN employees e on e.Id = te.EmployeeId
            WHERE t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        private const string AllEmployeeTimesheetHolidaysByPayrollPeriodQuery = $@"
            SELECT 
                tho.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                e.Id as EmployeeId,
                tho.TimesheetHeaderId AS TimesheetHeaderId,
                tho.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                tho.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                tho.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                null  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                null  as {nameof(TimesheetEntryDetails.JobNumber)},
                null  as {nameof(TimesheetEntryDetails.JobDescription)},
                null  as {nameof(TimesheetEntryDetails.LaborCode)},
                null  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                null  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                e.Department  as {nameof(TimesheetEntryDetails.Department)},
                tho.Description  as {nameof(TimesheetEntryDetails.Description)},
                null  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                tho.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
            FROM timesheetHoliday tho
            JOIN timesheets t on t.id = tho.TimesheetHeaderId
            JOIN employees e on (
                (e.IsSalaried = {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryWeeklyTimesheetParam}) 
                OR 
                (e.IsSalaried != {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryMonthlyTimesheetParam}))
            WHERE t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        public const string AllEmployeeTimesheetAllEntriesByPayrollPeriodQuery = $@"
            SELECT * 
            FROM ({AllEmployeeTimesheetEntriesByPayrollPeriodQuery}) entries
            UNION ALL ({AllEmployeeTimesheetHolidaysByPayrollPeriodQuery})
            ORDER BY 2
        ";
        #endregion

        #endregion

        #region TimesheetEntriesInPeriod
        private const string TimesheetEntriesInPeriodEmployeeIdParam = "@employeeId";
        private const string TimesheetEntriesInPeriodStartParam = "@start";
        private const string TimesheetEntriesInPeriodEndParam = "@end";

        public const string TimesheetEntriesInPeriod = $@"SELECT
            th.Id as {nameof(EmployeeTimesheetEntry.TimesheetHeaderId)},
            te.Id as {nameof(EmployeeTimesheetEntry.Id)},
            te.WorkDate AS {nameof(EmployeeTimesheetEntry.WorkDate)},
            te.PayrollCode AS {nameof(EmployeeTimesheetEntry.PayrollCode)},
            te.Hours AS {nameof(EmployeeTimesheetEntry.Quantity)}
            FROM timesheetEntry te 
            JOIN timesheets th on th.Id = te.TimesheetHeaderId
            WHERE te.EmployeeId = {TimesheetEntriesInPeriodEmployeeIdParam}
            AND WorkDate BETWEEN {TimesheetEntriesInPeriodStartParam} AND {TimesheetEntriesInPeriodEndParam}
            ORDER BY WorkDate
        ";
        #endregion
    }

    public class QueryTimesheet : BaseQuery, IQueryTimesheet
    {
        private readonly IDatabaseService _dbService;

        public QueryTimesheet(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByPayrollCode(string employeeId, string timesheetId)
        {
            var query = QueryTimesheetConstants.TimesheetSummaryByPayrollCodeQuery;
            var timesheetSummary = await _dbService.QueryAsync<EmployeeTimesheetDetailSummary>(query, new { employeeId, timesheetId });

            return timesheetSummary;
        }

        public async Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByDate(string employeeId, string timesheetId)
        {
            var query = QueryTimesheetConstants.TimesheetSummaryByDateQuery;
            var timesheetSummary = await _dbService.QueryAsync<EmployeeTimesheetDetailSummary>(query, new { employeeId, timesheetId });

            return timesheetSummary;
        }

        public async Task<EmployeeTimesheet?> GetEmployeeTimesheetDetails(string employeeId, string timesheetId)
        {
            var query = QueryTimesheetConstants.TimesheetDetailsQuery;
            var timesheet = (await _dbService.QueryAsync<EmployeeTimesheet>(query, new { employeeId, timesheetId }))?.FirstOrDefault();

            if(timesheet is null)
            {
                return null;
            }

            query = QueryTimesheetConstants.TimesheetDetailsEntriesQuery;
            var entries = await _dbService.QueryAsync<EmployeeTimesheetEntry>(query, new { employeeId, timesheetId });

            timesheet.Entries = entries;

            return timesheet;
        }

        public async Task<TimesheetReview> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page, int itemsPerPage)
        {
            var payrollPeriodParam = "@payrollPeriod";
            var employeeIdParam = "@employeeId";
            var departmentParam = "@department";

            var searchFilter = $@"
                { (string.IsNullOrEmpty(payrollPeriod) ? string.Empty : $"AND t.PayrollPeriod = {payrollPeriodParam}") }
                { (string.IsNullOrEmpty(employeeId) ? string.Empty : $"AND e.Id = {employeeIdParam}") }
                { (string.IsNullOrEmpty(department) ? string.Empty : $"AND e.Department = {departmentParam}") }
            ";

            var query = QueryTimesheetConstants.TimesheetReviewTotalQuery;
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQuerySearchFilterPlaceholder, searchFilter);

            var timesheetReview = (await _dbService.QueryAsync<TimesheetReview>(
                query,
                new
                {
                    employeeId,
                    payrollPeriod,
                    department,
                    isSalaried = true,
                    weeklyTimesheet = TimesheetType.WEEKLY,
                    monthlyTimesheet = TimesheetType.SALARLY
                }))
                ?.FirstOrDefault();

            if (timesheetReview is null)
            {
                return null;
            }

            var paginate = Paginate(page, itemsPerPage, string.Empty, QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheetOrderByClause);
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQuerySearchFilterPlaceholder, searchFilter);

            query = $@"WITH 
            {QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheetEntries},
            {QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheet}
            {QueryTimesheetConstants.TimesheetReviewQuery}
            ";
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQuerySearchFilterPlaceholder, searchFilter);
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQueryPaginatePlaceholder, paginate);


            var entries = await _dbService.QueryAsync<TimesheetEntryDetails>(
                query,
                new
                {
                    employeeId,
                    payrollPeriod,
                    department,
                    isSalaried = true,
                    weeklyTimesheet = TimesheetType.WEEKLY,
                    monthlyTimesheet = TimesheetType.SALARLY
                }
            );

            timesheetReview.Items = GroupEntriesByTimesheetAndEmployee(entries);

            return timesheetReview;
        }

        public async Task<EmployeeTimesheetHistory> GetEmployeeTimesheets(string employeeId, int page, int itemsPerPage)
        {
            var totalQuery = QueryTimesheetConstants.EmployeeTimesheetsTotalQuery;

            var query = QueryTimesheetConstants.EmployeeTimesheetsQuery;
            query = Paginate(page, itemsPerPage, query, QueryTimesheetConstants.EmployeeTimesheetsQueryOrderByClause);

            var timesheetHistory = (await _dbService.QueryAsync<EmployeeTimesheetHistory>(totalQuery, new { employeeId }))?.FirstOrDefault();
            if (timesheetHistory is not null)
            {
                var timesheets = await _dbService.QueryAsync<EmployeeTimesheet>(query, new { employeeId });
                timesheetHistory.Items = timesheets;
            }

            return timesheetHistory;
        }

        public async Task<AllEmployeesTimesheet<TEntry>?> GetAllEmployeeTimesheetByPayrollPeriod<TEntry>(string payrollPeriod)
        {
            var query = QueryTimesheetConstants.AllEmployeeTimesheetByPayrollPeriodQuery;
            var timesheet = (await _dbService.QueryAsync<AllEmployeesTimesheet<TEntry>>(query, new { payrollPeriod }))?.FirstOrDefault();
            
            if(timesheet is null)
            {
                return null;
            }

            query = typeof(TEntry) == typeof(ExternalTimesheetEntryDetails)
                ? QueryTimesheetConstants.ExternalAllEmployeeTimesheetAllEntriesByPayrollPeriodQuery
                : QueryTimesheetConstants.AllEmployeeTimesheetAllEntriesByPayrollPeriodQuery;

            var entries = await _dbService.QueryAsync<TEntry>(query, new { 
                payrollPeriod,
                isSalaried = true,
                weeklyTimesheet = TimesheetType.WEEKLY,
                monthlyTimesheet = TimesheetType.SALARLY
            });
            timesheet.Entries = entries;

            return timesheet;
        }

        private static List<EmployeeTimesheetWithTotals> GroupEntriesByTimesheetAndEmployee(List<TimesheetEntryDetails> entries)
        {
            return entries
                .GroupBy(e => new { e.EmployeeId, e.Fullname, e.TimesheetId, e.PayrollPeriod, e.Total, e.StartDate, e.EndDate, e.Status })
                .Select(g => new EmployeeTimesheetWithTotals
                {
                    EmployeeId = g.Key.EmployeeId,
                    Fullname = g.Key.Fullname,
                    TimesheetId = g.Key.TimesheetId,
                    PayrollPeriod = g.Key.PayrollPeriod,
                    Total = g.Key.Total,
                    StartDate = g.Key.StartDate,
                    EndDate = g.Key.EndDate,
                    Status = g.Key.Status,
                    Entries = g.Select(e => new EmployeeTimesheetEntry
                    {
                        Id = e.TimesheetEntryId,
                        WorkDate = e.WorkDate,
                        PayrollCode = e.PayrollCode,
                        Quantity = e.Quantity,
                        CustomerNumber = e.CustomerNumber,
                        JobNumber = e.JobNumber,
                        JobDescription = e.JobDescription,
                        LaborCode = e.LaborCode,
                        ServiceOrderNumber = e.ServiceOrderNumber,
                        ProfitCenterNumber = e.ProfitCenterNumber,
                        Department = e.Department,
                        Description = e.Description,
                        OutOffCountry = e.OutOffCountry,
                        Status = e.TimesheetEntryStatus
                    })
                }).ToList();
        }

        public async Task<IEnumerable<EmployeeTimesheetEntry>> GetEmployeeTimesheetEntriesInPeriod(string employeeId, DateTime start, DateTime end)
        {
            var query = QueryTimesheetConstants.TimesheetEntriesInPeriod;
            var entries = await _dbService.QueryAsync<EmployeeTimesheetEntry>(query, new { employeeId, start, end });

            return entries;
        }
    }
}