using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastruture.ReadModel.Queries;
using Timesheet.Models.Referential;

namespace Timesheet.Infrastructure.Persistence.Queries
{

    public static class QueryTimesheetConstants
    {
        #region EmployeeTimesheetSummaryBy
        private const string TimesheetSummaryQueryEmployeeIdParam = "@employeeId";
        private const string TimesheetSummaryQueryTimesheetIdParam = "@timesheetId";

        private const string TimesheetSummaryByQuery = $@"
            SUM(te.Hours) as {nameof(EmployeeTimesheetDetailSummary.Hours)}
            FROM employees e
            JOIN timesheetEntry te on e.Id = te.EmployeeId
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            JOIN payrollTypes pt on pt.numId = te.PayrollCodeId
            Where e.Id = {TimesheetSummaryQueryEmployeeIdParam} AND t.Id = {TimesheetSummaryQueryTimesheetIdParam}
        ";

        public const string TimesheetSummaryByPayrollCodeQuery = $@"SELECT
            pt.PayrollCode  as {nameof(EmployeeTimesheetDetailSummary.PayrollCode)},
            te.PayrollCodeId  as {nameof(EmployeeTimesheetDetailSummary.PayrollCodeId)},
            {TimesheetSummaryByQuery}
            GROUP BY pt.PayrollCode, te.PayrollCodeId
        ";

        public const string TimesheetSummaryByDateQuery = $@"SELECT
            te.WorkDate  as {nameof(EmployeeTimesheetDetailSummary.WorkDate)},
            {TimesheetSummaryByQuery}
            GROUP BY te.WorkDate
        ";
        #endregion

        #region FirstTimesheetBefore
        private const string FirstTimesheetBeforeQueryEmployeeIdParam = "@employeeId";
        private const string FirstTimesheetBeforeQueryTimesheetTypeParam = "@timesheetType";
        private const string FirstTimesheetBeforeQueryCurrentTimesheetStartDate = "@currentTimesheetStartDate";

        public const string FirstTimesheetBeforeQuery = $@"
            SELECT TOP 1 t.id
            FROM timesheets t
            JOIN timesheetEntry te on t.id = te.TimesheetHeaderId AND te.EmployeeId = {FirstTimesheetBeforeQueryEmployeeIdParam}
            WHERE t.type = {FirstTimesheetBeforeQueryTimesheetTypeParam} AND t.EndDate < {FirstTimesheetBeforeQueryCurrentTimesheetStartDate}
            ORDER BY t.EndDate DESC, id DESC
        ";

        #endregion

        #region FirstTimesheetAfter

        private const string FirstTimesheetAfterQueryEmployeeIdParam = "@employeeId";
        private const string FirstTimesheetAfterQueryTimesheetTypeParam = "@timesheetType";
        private const string FirstTimesheetAfterQueryCurrentTimesheetEndDate = "@currentTimesheetEndDate";

        public const string FirstTimesheetAfterQuery = $@"
            SELECT TOP 1 t.id
            FROM timesheets t
            JOIN timesheetEntry te on t.id = te.TimesheetHeaderId AND te.EmployeeId = {FirstTimesheetAfterQueryEmployeeIdParam}
            WHERE t.type = {FirstTimesheetAfterQueryTimesheetTypeParam} AND t.StartDate > {FirstTimesheetAfterQueryCurrentTimesheetEndDate}
            ORDER BY t.StartDate ASC, id ASC
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
            te.Id AS {nameof(EmployeeTimesheetEntry.Id)},
            te.CreatedDate  as {nameof(EmployeeTimesheetEntry.CreatedDate)},
            te.ModifiedDate  as {nameof(EmployeeTimesheetEntry.ModifiedDate)},
            te.WorkDate  as {nameof(EmployeeTimesheetEntry.WorkDate)},
            te.PayrollCodeId  as {nameof(EmployeeTimesheetEntry.PayrollCodeId)},
            pt.PayrollCode  as {nameof(EmployeeTimesheetEntry.PayrollCode)},
            te.Hours  as {nameof(EmployeeTimesheetEntry.Quantity)},
            te.JobNumber  as {nameof(EmployeeTimesheetEntry.JobNumber)},
            te.JobDescription  as {nameof(EmployeeTimesheetEntry.JobDescription)},
            te.LaborCode  as {nameof(EmployeeTimesheetEntry.LaborCode)},
            te.ServiceOrderNumber  as {nameof(EmployeeTimesheetEntry.ServiceOrderNumber)},
            COALESCE(te.ProfitCenterNumber, e.DefaultProfitCenter)  as {nameof(EmployeeTimesheetEntry.ProfitCenterNumber)},
            e.Department  as {nameof(EmployeeTimesheetEntry.Department)},
            CASE WHEN pt.Category = 2 THEN te.Description 
            ELSE ptdesc.PayrollCode END AS {nameof(EmployeeTimesheetEntry.Description)}, 
            te.OutOffCountry  as {nameof(EmployeeTimesheetEntry.OutOffCountry)},
            te.Status  as {nameof(EmployeeTimesheetEntry.Status)},
            te.IsDeletable  as {nameof(EmployeeTimesheetEntry.IsDeletable)},
            te.IsTimeoff  as {nameof(EmployeeTimesheetEntry.IsTimeoff)}
            FROM timesheetEntry te
            JOIN payrollTypes pt on pt.numId = te.PayrollCodeId
            JOIN payrollTypes ptdesc on ptdesc.numId = CAST(te.PayrollCodeId AS INT)  
            JOIN employees e on e.id = te.employeeId
            WHERE employeeId = {TimesheetDetailsQueryEmployeeIdParam} AND timesheetHeaderId = {TimesheetDetailsQueryTimesheetIdParam}
            ORDER BY te.WorkDate
        ";
        #endregion

        #region TimesheetReview
        private const string TimesheetReviewQueryIsSalariedParam = "@isSalaried";
        private const string TimesheetReviewQueryWeeklyTimesheetParam = "@weeklyTimesheet";
        private const string TimesheetReviewQueryMonthlyTimesheetParam = "@monthlyTimesheet";
        private const string TimesheetReviewQueryHolidayPayrollCodeIdParam = "@holidayPayrollCodeId";
        private const string TimesheetReviewQueryOvertimePayrollCodeIdParam = "@overtimePayrollCodeId";

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
                        LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = e.Id
                        WHERE @teamClause tex.Id is null  
                        { TimesheetReviewQuerySearchFilterPlaceholder }
                    ) t
                    UNION ALL(
                        SELECT e.id AS {nameof(TimesheetEntryDetails.EmployeeId)}, 
                        t.id AS {nameof(TimesheetEntryDetails.TimesheetId)}, 
                        tho.Hours 
                        FROM timesheetHoliday tho
                        JOIN timesheets t ON t.id = tho.TimesheetHeaderId
                        JOIN employees e ON (
                            (e.IsSalaried = {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryWeeklyTimesheetParam}) 
                            OR 
                            (e.IsSalaried != {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryMonthlyTimesheetParam}))
                        LEFT JOIN TimesheetException ho ON ho.TimesheetEntryId = tho.Id And ho.EmployeeId = e.Id
                        WHERE @teamClause ho.Id is null
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
                    te.PayrollCodeId  as {nameof(TimesheetEntryDetails.PayrollCodeId)},
                    pt.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                    te.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                    te.CustomerNumber  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                    te.JobNumber  as {nameof(TimesheetEntryDetails.JobNumber)},
                    te.JobDescription  as {nameof(TimesheetEntryDetails.JobDescription)},
                    te.LaborCode  as {nameof(TimesheetEntryDetails.LaborCode)},
                    te.ServiceOrderNumber  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                    COALESCE(te.ProfitCenterNumber, e.DefaultProfitCenter)  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                    e.Department  as {nameof(TimesheetEntryDetails.Department)},
                    te.Description  as {nameof(TimesheetEntryDetails.Description)},
                    te.OutOffCountry  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                    te.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
                    FROM timesheetEntry te
                    JOIN timesheets t on t.id = te.TimesheetHeaderId
                    JOIN employees e on e.Id = te.EmployeeId
                    JOIN payrollTypes pt on pt.numId = te.PayrollCodeId
                    LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = e.Id
                    WHERE @teamClause tex.Id is null 
                    { TimesheetReviewQuerySearchFilterPlaceholder }
                ) entries
                UNION ALL
                (SELECT 
                    tho.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                    e.Id as EmployeeId,
                    tho.TimesheetHeaderId AS TimesheetHeaderId,
                    tho.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                    {TimesheetReviewQueryHolidayPayrollCodeIdParam} as {nameof(TimesheetEntryDetails.PayrollCodeId)},
                    pt.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                    tho.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                    null  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                    null  as {nameof(TimesheetEntryDetails.JobNumber)},
                    null  as {nameof(TimesheetEntryDetails.JobDescription)},
                    null  as {nameof(TimesheetEntryDetails.LaborCode)},
                    null  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                    e.defaultProfitCenter  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                    e.Department  as {nameof(TimesheetEntryDetails.Department)},
                    tho.Description  as {nameof(TimesheetEntryDetails.Description)},
                    null  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                    tho.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
                    FROM timesheetHoliday tho
                    JOIN timesheets t on t.id = tho.TimesheetHeaderId
                    JOIN payrollTypes pt on pt.numId = {TimesheetReviewQueryHolidayPayrollCodeIdParam}
                    JOIN employees e on (
                        (e.IsSalaried = {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryWeeklyTimesheetParam}) 
                        OR 
                        (e.IsSalaried != {TimesheetReviewQueryIsSalariedParam} AND t.type = {TimesheetReviewQueryMonthlyTimesheetParam}))
                    LEFT JOIN TimesheetException ho ON ho.TimesheetEntryId = tho.Id And ho.EmployeeId = e.Id
                    WHERE @teamClause ho.Id is null
                    { TimesheetReviewQuerySearchFilterPlaceholder }
                )
            )";

        public const string TimesheetReviewQueryEmployeeTimesheetOrderByClause = $@"e.{nameof(TimesheetEntryDetails.Fullname)}";

        public const string TimesheetReviewQueryEmployeeTimesheet = $@"employeeTimesheet AS
            (SELECT
                e.Id AS {nameof(TimesheetEntryDetails.EmployeeId)},
                e.Fullname AS {nameof(TimesheetEntryDetails.Fullname)},
                e.DefaultProfitCenter AS {nameof(TimesheetEntryDetails.DefaultProfitCenter)},
                t.Id AS {nameof(TimesheetEntryDetails.TimesheetId)},
                t.PayrollPeriod AS {nameof(TimesheetEntryDetails.PayrollPeriod)},
                SUM(CASE WHEN te.{nameof(TimesheetEntryDetails.PayrollCodeId)} = {TimesheetReviewQueryOvertimePayrollCodeIdParam} 
                    THEN te.{nameof(TimesheetEntryDetails.Quantity)} else 0 END)
                    AS {nameof(TimesheetEntryDetails.Overtime)},
                SUM(te.{nameof(TimesheetEntryDetails.Quantity)}) AS {nameof(TimesheetEntryDetails.Total)},
                t.StartDate AS {nameof(TimesheetEntryDetails.StartDate)},
                t.EndDate AS {nameof(TimesheetEntryDetails.EndDate)},
                t.Status AS {nameof(TimesheetEntryDetails.Status)}
                FROM employees e
                JOIN employeeTimesheetEntries te on e.Id = te.EmployeeId
                JOIN timesheets t on t.id = te.TimesheetHeaderId
                WHERE @teamClause 1=1 
                { TimesheetReviewQuerySearchFilterPlaceholder }
                GROUP BY e.Id, e.Fullname, t.Id, t.PayrollPeriod, t.StartDate, t.EndDate, t.Status, e.DefaultProfitCenter
                {TimesheetReviewQueryPaginatePlaceholder}
            )
        ";

        public const string TimesheetReviewQuery = $@"SELECT 
            et.{nameof(TimesheetEntryDetails.EmployeeId)},
            et.{nameof(TimesheetEntryDetails.Fullname)},
            et.{nameof(TimesheetEntryDetails.DefaultProfitCenter)},
            et.{nameof(TimesheetEntryDetails.TimesheetId)},
            et.{nameof(TimesheetEntryDetails.PayrollPeriod)},
            et.{nameof(TimesheetEntryDetails.Overtime)},
            et.{nameof(TimesheetEntryDetails.Total)},
            et.{nameof(TimesheetEntryDetails.StartDate)},
            et.{nameof(TimesheetEntryDetails.EndDate)},
            et.{nameof(TimesheetEntryDetails.Status)},
            te.{nameof(TimesheetEntryDetails.TimesheetEntryId)},
            te.{nameof(TimesheetEntryDetails.WorkDate)},
            te.{nameof(TimesheetEntryDetails.PayrollCodeId)},
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
            ORDER BY et.{nameof(TimesheetEntryDetails.Fullname)}, te.{nameof(TimesheetEntryDetails.WorkDate)}
        ";

        #endregion

        #region EmployeeTimesheets
        private const string EmployeeTimesheetsQueryEmployeeIdParam = "@employeeId";

        private const string EmployeeTimesheetsQueryFromClause = $@"FROM employees e
            JOIN timesheetEntry te on e.Id = te.EmployeeId
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            Where e.Id = {EmployeeTimesheetsQueryEmployeeIdParam}
            GROUP BY e.Id, e.Fullname, t.Id, t.PayrollPeriod, t.CreatedDate, t.ModifiedDate, t.StartDate, t.EndDate, t.status";

        public const string EmployeeTimesheetsQueryOrderByClause = $@"t.{nameof(EmployeeTimesheet.StartDate)} DESC";

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
        private const string AllEmployeeTimesheetByPayrollPeriodQueryHolidayPayrollCodeIdParam = "@holidayPayrollCodeId";
        private const string AllEmployeeTimesheetByPayrollPeriodQueryUnpaidPayrollCodeIdParam = "@unpaidPayrollCodeId";
        private const string AllEmployeeTimesheetByPayrollPeriodQueryApprovedEntryStatus = "@approvedStatus";


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
                ('""""""' +  RIGHT('0000' + CAST(te.EmployeeId AS varchar(4)), 4 )+ '""""""') AS {nameof(ExternalTimesheetEntryDetails.No)},
                pt.ExternalCode AS {nameof(ExternalTimesheetEntryDetails.PayrollCodeId)},
                te.Hours  as {nameof(ExternalTimesheetEntryDetails.Hours)},
                ('""""""'+ cast((coalesce(te.ProfitCenterNumber, e.defaultProfitCenter)) as varchar)+'""""""') as {nameof(ExternalTimesheetEntryDetails.CC1)},
                IIF(COALESCE(JobNumber, ServiceOrderNumber) is null or LEN(COALESCE(JobNumber, ServiceOrderNumber)) = 0, '', ('""""""'+ Left(coalesce(te.JobNumber, te.ServiceOrderNumber),10) +'""""""')) AS {nameof(ExternalTimesheetEntryDetails.Job_Code)},
                ('""""""'+Convert(varchar,WorkDate,1)+'""""""') AS {nameof(ExternalTimesheetEntryDetails.Begin_Date)},
                ('""""""'+Convert(varchar,WorkDate,1)+'""""""') AS {nameof(ExternalTimesheetEntryDetails.End_Date)}

                --concat(replace(convert(varchar, te.WorkDate, 10),'-','/'), ' 1:01am') AS {nameof(ExternalTimesheetEntryDetails.Begin_Date)},
                --concat(replace(convert(varchar, te.WorkDate, 10),'-','/'), ' 23:49:59') AS {nameof(ExternalTimesheetEntryDetails.End_Date)},

            FROM timesheetEntry te
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            JOIN employees e on e.Id = te.EmployeeId
            JOIN PayrollTypes pt on pt.numId = te.PayrollCodeId
            LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = e.Id
            WHERE tex.Id is null AND te.Status = {AllEmployeeTimesheetByPayrollPeriodQueryApprovedEntryStatus} AND
            t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
            AND pt.numId != {AllEmployeeTimesheetByPayrollPeriodQueryUnpaidPayrollCodeIdParam}
        ";

        private const string ExternalAllEmployeeTimesheetHolidaysByPayrollPeriodQuery = $@"
            SELECT 
                ('""""""' +  RIGHT('0000' + CAST(e.Id AS varchar(4)), 4 )+ '""""""') AS {nameof(ExternalTimesheetEntryDetails.No)},
                pt.ExternalCode AS {nameof(ExternalTimesheetEntryDetails.PayrollCodeId)},
                tho.Hours  as {nameof(ExternalTimesheetEntryDetails.Hours)},
                (e.defaultProfitCenter) as {nameof(ExternalTimesheetEntryDetails.CC1)},
                ('') as {nameof(ExternalTimesheetEntryDetails.Job_Code)},
                (''+Convert(varchar,tho.WorkDate,1)+'""""""') AS {nameof(ExternalTimesheetEntryDetails.Begin_Date)},
                (''+Convert(varchar,tho.WorkDate,1)+'""""""') AS {nameof(ExternalTimesheetEntryDetails.End_Date)}
            FROM timesheetHoliday tho
            JOIN timesheets t on t.id = tho.TimesheetHeaderId
            JOIN PayrollTypes pt on pt.numId = {AllEmployeeTimesheetByPayrollPeriodQueryHolidayPayrollCodeIdParam}
            JOIN employees e on (
                (e.IsSalaried = {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryWeeklyTimesheetParam}) 
                OR 
                (e.IsSalaried != {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryMonthlyTimesheetParam}))
            LEFT JOIN TimesheetException ho ON ho.TimesheetEntryId = tho.Id And ho.EmployeeId = e.Id
            WHERE ho.Id is null
            AND t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
        ";

        public const string ExternalAllEmployeeTimesheetAllEntriesByPayrollPeriodQuery = $@"
            SELECT * 
            FROM ({ExternalAllEmployeeTimesheetEntriesByPayrollPeriodQuery}) entries
            UNION ALL ({ExternalAllEmployeeTimesheetHolidaysByPayrollPeriodQuery})
            ORDER BY 2
        ";

        public const string ExternalAllEmployeeTimesheetEntriesWithoutHolidaysByPayrollPeriodQuery = $@"
            SELECT * 
            FROM ({ExternalAllEmployeeTimesheetEntriesByPayrollPeriodQuery}) entries
            ORDER BY 2
        ";
        #endregion

        #region Export web
        private const string AllTimesheetEntriesBySearchCriteriaQueryPayrollPeriodParam = "@payrollPeriod";
        private const string AllTimesheetEntriesBySearchCriteriaQueryDepartmentParam = "@department";
        private const string AllTimesheetEntriesBySearchCriteriaQueryEmployeeIdParam = "@employeeId";

        public const string AllTimesheetEntriesBySearchCriteriaQueryDepartmentWhereClause = $@"
            AND e.Department = {AllTimesheetEntriesBySearchCriteriaQueryDepartmentParam}
        ";

        public const string AllTimesheetEntriesBySearchCriteriaQueryEmployeeWhereClause = $@"
            AND e.Id = {AllTimesheetEntriesBySearchCriteriaQueryEmployeeIdParam}
        ";

        private const string AllTimesheetEntriesBySearchCriteriaQuery = $@"
            SELECT 
                te.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                te.EmployeeId as {nameof(TimesheetEntryDetails.EmployeeId)},
                e.Fullname as {nameof(TimesheetEntryDetails.Fullname)},
                te.TimesheetHeaderId AS TimesheetHeaderId,
                te.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                te.PayrollCodeId  as {nameof(TimesheetEntryDetails.PayrollCodeId)},
                pt.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                te.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                te.CustomerNumber  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                te.JobNumber  as {nameof(TimesheetEntryDetails.JobNumber)},
                te.JobDescription  as {nameof(TimesheetEntryDetails.JobDescription)},
                te.LaborCode  as {nameof(TimesheetEntryDetails.LaborCode)},
                te.ServiceOrderNumber  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                coalesce(te.ProfitCenterNumber, e.defaultProfitCenter)  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                e.Department  as {nameof(TimesheetEntryDetails.Department)},
                te.Description  as {nameof(TimesheetEntryDetails.Description)},
                te.OutOffCountry  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                te.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
            FROM timesheetEntry te
            JOIN timesheets t on t.id = te.TimesheetHeaderId
            JOIN employees e on e.Id = te.EmployeeId
            JOIN payrollTypes pt on pt.numId =  te.PayrollCodeId
            LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = e.Id
            WHERE tex.Id is null AND 
            t.payrollPeriod = {AllTimesheetEntriesBySearchCriteriaQueryPayrollPeriodParam}
            @AllTimesheetEntriesBySearchCriteriaQueryDepartmentWhereClause
            @AllTimesheetEntriesBySearchCriteriaQueryEmployeeWhereClause
        ";

        private const string AllEmployeeTimesheetHolidaysBySearchCriteriaQuery = $@"
            SELECT 
                tho.Id AS {nameof(TimesheetEntryDetails.TimesheetEntryId)},
                e.Id as {nameof(TimesheetEntryDetails.EmployeeId)},
                e.Fullname as {nameof(TimesheetEntryDetails.Fullname)},
                tho.TimesheetHeaderId AS TimesheetHeaderId,
                tho.WorkDate  as {nameof(TimesheetEntryDetails.WorkDate)},
                3  as {nameof(TimesheetEntryDetails.PayrollCodeId)},
                pt.PayrollCode  as {nameof(TimesheetEntryDetails.PayrollCode)},
                tho.Hours  as {nameof(TimesheetEntryDetails.Quantity)},
                null  as {nameof(TimesheetEntryDetails.CustomerNumber)},
                null  as {nameof(TimesheetEntryDetails.JobNumber)},
                null  as {nameof(TimesheetEntryDetails.JobDescription)},
                null  as {nameof(TimesheetEntryDetails.LaborCode)},
                null  as {nameof(TimesheetEntryDetails.ServiceOrderNumber)},
                e.defaultProfitCenter  as {nameof(TimesheetEntryDetails.ProfitCenterNumber)},
                e.Department  as {nameof(TimesheetEntryDetails.Department)},
                tho.Description  as {nameof(TimesheetEntryDetails.Description)},
                null  as {nameof(TimesheetEntryDetails.OutOffCountry)},
                tho.Status  as {nameof(TimesheetEntryDetails.TimesheetEntryStatus)}
            FROM timesheetHoliday tho
            JOIN timesheets t on t.id = tho.TimesheetHeaderId
            JOIN payrollTypes pt on pt.numId =  {AllEmployeeTimesheetByPayrollPeriodQueryHolidayPayrollCodeIdParam}
            JOIN employees e on (
                (e.IsSalaried = {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryWeeklyTimesheetParam}) 
                OR 
                (e.IsSalaried != {AllEmployeeTimesheetByPayrollPeriodQueryIsSalariedParam} AND t.type = {AllEmployeeTimesheetByPayrollPeriodQueryMonthlyTimesheetParam}))
            LEFT JOIN TimesheetException ho ON ho.TimesheetEntryId = tho.Id And ho.EmployeeId = e.Id
            WHERE ho.Id is null AND t.payrollPeriod = {AllEmployeeTimesheetByPayrollPeriodQueryPayrollPeriodParam}
            @{nameof(AllTimesheetEntriesBySearchCriteriaQueryDepartmentWhereClause)}
            @{nameof(AllTimesheetEntriesBySearchCriteriaQueryEmployeeWhereClause)}
        ";

        public const string AllEmployeeTimesheetAllEntriesByPayrollPeriodQuery = $@"
            SELECT * 
            FROM ({AllTimesheetEntriesBySearchCriteriaQuery}) entries
            UNION ALL ({AllEmployeeTimesheetHolidaysBySearchCriteriaQuery})
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
            te.PayrollCodeId AS {nameof(EmployeeTimesheetEntry.PayrollCodeId)},
            pt.PayrollCode AS {nameof(EmployeeTimesheetEntry.PayrollCode)},
            te.Hours AS {nameof(EmployeeTimesheetEntry.Quantity)}
            FROM timesheetEntry te 
            JOIN timesheets th on th.Id = te.TimesheetHeaderId
            JOIN payrollTypes pt on pt.numId = te.PayrollCodeId
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

        public async Task<string> GetFirstTimesheetBefore(string employeeId, DateTime currentTimesheetStartDate, TimesheetType timesheetType)
        {
            var query = QueryTimesheetConstants.FirstTimesheetBeforeQuery;
            var previousTimesheetId = await _dbService.QueryAsync<string>(query, new { 
                employeeId, 
                currentTimesheetStartDate,
                timesheetType
            });

            return previousTimesheetId.FirstOrDefault();
        }

        public async Task<string> GetFirstTimesheetAfter(string employeeId, DateTime currentTimesheetEndDate, TimesheetType timesheetType)
        {
            var query = QueryTimesheetConstants.FirstTimesheetAfterQuery;
            var nextTimesheetId = await _dbService.QueryAsync<string>(query, new
            {
                employeeId,
                currentTimesheetEndDate,
                timesheetType
            });

            return nextTimesheetId?.FirstOrDefault();
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

            timesheet.NextTimesheetId = await this.GetFirstTimesheetAfter(timesheet.EmployeeId, timesheet.EndDate, timesheet.Type);
            timesheet.PreviousTimesheetId = await this.GetFirstTimesheetBefore(timesheet.EmployeeId, timesheet.StartDate, timesheet.Type);

            return timesheet;
        }

        public async Task<TimesheetReview> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page, int itemsPerPage, string managerId = null)
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

            query = AddWhereClauseForDirectReports(managerId, false, query, "e.id", whereKey: "", "@teamClause", addAnd: ADD_AND.AND_AFTER);

            var timesheetReview = (await _dbService.QueryAsync<TimesheetReview>(
                query,
                new
                {
                    employeeId,
                    payrollPeriod,
                    department,
                    isSalaried = true,
                    weeklyTimesheet = TimesheetType.WEEKLY,
                    monthlyTimesheet = TimesheetType.SALARLY,
                    approverId = managerId
                }))
                ?.FirstOrDefault();

            if (timesheetReview is null)
            {
                return null;
            }

            var paginate = Paginate(page, itemsPerPage, string.Empty, QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheetOrderByClause);
            //query = query.Replace(QueryTimesheetConstants.TimesheetReviewQuerySearchFilterPlaceholder, searchFilter);

            query = $@"WITH 
            {QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheetEntries},
            {QueryTimesheetConstants.TimesheetReviewQueryEmployeeTimesheet}
            {QueryTimesheetConstants.TimesheetReviewQuery}
            ";
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQuerySearchFilterPlaceholder, searchFilter);
            query = query.Replace(QueryTimesheetConstants.TimesheetReviewQueryPaginatePlaceholder, paginate);

            query = AddWhereClauseForDirectReports(managerId, false, query, "e.id", whereKey: "", "@teamClause", addAnd: ADD_AND.AND_AFTER);

            var entries = await _dbService.QueryAsync<TimesheetEntryDetails>(
                query,
                new
                {
                    employeeId,
                    payrollPeriod,
                    department,
                    isSalaried = true,
                    weeklyTimesheet = TimesheetType.WEEKLY,
                    monthlyTimesheet = TimesheetType.SALARLY,
                    holidayPayrollCodeId = (int)TimesheetFixedPayrollCodeEnum.HOLIDAY,
                    overtimePayrollCodeId = (int)TimesheetFixedPayrollCodeEnum.OVERTIME,
                    approverId = managerId
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

        public async Task<AllEmployeesTimesheet<TimesheetEntryDetails>> GetAllTimesheetEntriesByPayrollPeriodAndCriteria(string payrollPeriod, string? department, string? employeeId)
        {
            var query = QueryTimesheetConstants.AllEmployeeTimesheetByPayrollPeriodQuery;
            var timesheet = (await _dbService.QueryAsync<AllEmployeesTimesheet<TimesheetEntryDetails>>(query, new { payrollPeriod }))?.FirstOrDefault();
            
            if(timesheet is null)
            {
                return null;
            }

            query = QueryTimesheetConstants.AllEmployeeTimesheetAllEntriesByPayrollPeriodQuery;
            query = query.Replace($"@{nameof(QueryTimesheetConstants.AllTimesheetEntriesBySearchCriteriaQueryDepartmentWhereClause)}", department!=null ? QueryTimesheetConstants.AllTimesheetEntriesBySearchCriteriaQueryDepartmentWhereClause : string.Empty);
            query = query.Replace($"@{nameof(QueryTimesheetConstants.AllTimesheetEntriesBySearchCriteriaQueryEmployeeWhereClause)}", employeeId!=null ? QueryTimesheetConstants.AllTimesheetEntriesBySearchCriteriaQueryEmployeeWhereClause : string.Empty);
           

            var entries = await _dbService.QueryAsync<TimesheetEntryDetails>(query, new { 
                payrollPeriod,
                department,
                employeeId,
                isSalaried = true,
                weeklyTimesheet = TimesheetType.WEEKLY,
                monthlyTimesheet = TimesheetType.SALARLY,
                holidayPayrollCodeId = (int) TimesheetFixedPayrollCodeEnum.HOLIDAY
            });

            timesheet.Entries = entries;

            return timesheet;
        }

        public async Task<AllEmployeesTimesheet<ExternalTimesheetEntryDetails>> GetAllTimesheetEntriesByPayrollPeriod(string payrollPeriod, bool ignoreHolidays)
        {
            var query = QueryTimesheetConstants.AllEmployeeTimesheetByPayrollPeriodQuery;
            var timesheet = (await _dbService.QueryAsync<AllEmployeesTimesheet<ExternalTimesheetEntryDetails>>(query, new { payrollPeriod }))?.FirstOrDefault();

            if (timesheet is null)
            {
                return null;
            }

            query = ignoreHolidays
                ? QueryTimesheetConstants.ExternalAllEmployeeTimesheetEntriesWithoutHolidaysByPayrollPeriodQuery
                : QueryTimesheetConstants.ExternalAllEmployeeTimesheetAllEntriesByPayrollPeriodQuery;

            var entries = await _dbService.QueryAsync<ExternalTimesheetEntryDetails>(query, new
            {
                payrollPeriod,
                isSalaried = true,
                weeklyTimesheet = TimesheetType.WEEKLY,
                monthlyTimesheet = TimesheetType.SALARLY,
                holidayPayrollCodeId = (int)TimesheetFixedPayrollCodeEnum.HOLIDAY,
                unpaidPayrollCodeId = (int)TimesheetFixedPayrollCodeEnum.UNPAID,
                approvedStatus = TimesheetEntryStatus.APPROVED
            });
            timesheet.Entries = entries;

            return timesheet;
        }
        
        private static List<EmployeeTimesheetWithTotals> GroupEntriesByTimesheetAndEmployee(List<TimesheetEntryDetails> entries)
        {
            return entries
                .GroupBy(e => new { e.EmployeeId, e.Fullname, e.DefaultProfitCenter, e.TimesheetId, e.PayrollPeriod, e.Total, e.Overtime, e.StartDate, e.EndDate, e.Status })
                .Select(g => new EmployeeTimesheetWithTotals
                {
                    EmployeeId = g.Key.EmployeeId,
                    Fullname = g.Key.Fullname,
                    DefaultProfitCenter = g.Key.DefaultProfitCenter,
                    TimesheetId = g.Key.TimesheetId,
                    PayrollPeriod = g.Key.PayrollPeriod,
                    Total = g.Key.Total,
                    Overtime = g.Key.Overtime,
                    StartDate = g.Key.StartDate,
                    EndDate = g.Key.EndDate,
                    Status = g.Key.Status,
                    Entries = g.Select(e => new EmployeeTimesheetEntry
                    {
                        Id = e.TimesheetEntryId,
                        WorkDate = e.WorkDate,
                        PayrollCode = e.PayrollCode,
                        Quantity = e.Quantity,
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