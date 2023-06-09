using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Models.Referential;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal partial class PayrollRepository : IRepository<TimesheetHeader>
    {
        private const string TimesheetEntryTable = "TimesheetEntry";

        public async Task Delete(string id)
        {
            var timesheetEntryId = "@timesheetEntryId";

            var query = $@"DELETE 
                        FROM {TimesheetEntryTable} 
                        WHERE {nameof(TimesheetEntry.Id)} = {timesheetEntryId}";

            if (id is not null)
            {
                _databaseService.ExecuteAsync(query, new
                {
                    timesheetEntryId = id,
                }).Wait();
            }
        }

        public IEnumerable<TimesheetHeader> GetRecents()
        {
            var payrollCodeIdRegular = "@payrollCodeIdRegular";
            var payrollCodeIdOvertime = "@payrollCodeIdOvertime";

            var query = $@"SELECT id
                FROM {TimesheetEntryTable} 
                WHERE {nameof(TimesheetEntry.WorkDate)} > (GetDate() - 13)
                AND ({nameof(TimesheetEntry.PayrollCodeId)} = {payrollCodeIdRegular} OR {nameof(TimesheetEntry.PayrollCodeId)} = {payrollCodeIdOvertime})
                ORDER BY {nameof(TimesheetEntry.WorkDate)}";

            var entries = _databaseService.Query<TimesheetEntry>(query, new
            {
                payrollCodeIdRegular = TimesheetFixedPayrollCodeEnum.REGULAR,
                payrollCodeIdOvertime = TimesheetFixedPayrollCodeEnum.OVERTIME,
            });

            return entries.Select(e =>
            {
                var timesheet = new TimesheetHeader("dummy");//Not good but need a wrapper
                timesheet.AddTimesheetEntry(e);
                return timesheet;
            }).ToList();
        }

        public async Task UpSertEntry(TimesheetHeader timesheet)
        {
            var timesheetId = "@timesheetId";
            var timesheetHeaderId = "@timesheetHeaderId";
            var timesheetEmployeeId = "@timesheetEmployeeId";
            var timesheetWorkDate = "@timesheetWorkDate";
            var timesheetPayrollCodeId = "@timesheetPayrollCodeId";
            var timesheetHours = "@timesheetHours";
            var timesheetDescription = "@timesheetDescription";
            var timesheetServiceOrderNumber = "@timesheetServiceOrderNumber";
            var timesheetJobNumber = "@timesheetJobNumber";
            var timesheetJobTaskNumber = "@timesheetJobTaskNumber";
            var timesheetProfitCenter = "@timesheetProfitCenter";
            var timesheetCreatedDate = "@timesheetCreatedDate";
            var timesheetModifiedDate = "@timesheetModifiedDate";
            var timesheetUpdatedBy = "@timesheetUpdatedBy";
            var timesheetStatus = "@timesheetStatus";
            var timesheetIsTimeoff = "@timesheetIsTimeoff";
            var timesheetIsDeletable = "@timesheetIsDeletable";

            var updates = $@"
            {nameof(TimesheetEntry.EmployeeId)} = {timesheetEmployeeId},
            {nameof(TimesheetEntry.WorkDate)} = {timesheetWorkDate},
            {nameof(TimesheetHeader)}{nameof(TimesheetHeader.Id)} = {timesheetHeaderId},
            {nameof(TimesheetEntry.PayrollCodeId)} = {timesheetPayrollCodeId},
            {nameof(TimesheetEntry.Hours)} = {timesheetHours},
            {nameof(TimesheetEntry.Description)} = {timesheetDescription},
            {nameof(TimesheetEntry.ServiceOrderNumber)} = {timesheetServiceOrderNumber},
            {nameof(TimesheetEntry.JobNumber)} = {timesheetJobNumber},
            {nameof(TimesheetEntry.JobTaskNumber)} = {timesheetJobTaskNumber},
            {nameof(TimesheetEntry.ProfitCenterNumber)} = {timesheetProfitCenter},
            {nameof(TimesheetEntry.ModifiedDate)} = {timesheetModifiedDate},
            {nameof(TimesheetEntry.UpdatedBy)} = {timesheetUpdatedBy}
            ";

            var insertColums = $@"
            {nameof(TimesheetEntry.Id)},
            {nameof(TimesheetHeader)}{nameof(TimesheetHeader.Id)},
            { nameof(TimesheetEntry.EmployeeId)},
            {nameof(TimesheetEntry.WorkDate)},
            {nameof(TimesheetEntry.PayrollCodeId)},
            {nameof(TimesheetEntry.Hours)},
            {nameof(TimesheetEntry.Description)},
            {nameof(TimesheetEntry.ServiceOrderNumber)},
            {nameof(TimesheetEntry.JobNumber)},
            {nameof(TimesheetEntry.JobTaskNumber)},
            {nameof(TimesheetEntry.ProfitCenterNumber)},
            {nameof(TimesheetEntry.CreatedDate)},
            {nameof(TimesheetEntry.ModifiedDate)},
            {nameof(TimesheetEntry.UpdatedBy)},
            {nameof(TimesheetEntry.Status)},
            {nameof(TimesheetEntry.IsTimeoff)},            
            {nameof(TimesheetEntry.IsDeletable)}

            ";

            var insertValues = $@"
            {timesheetId},
            {timesheetHeaderId},
            {timesheetEmployeeId},
            {timesheetWorkDate},
            {timesheetPayrollCodeId},
            {timesheetHours},
            {timesheetDescription},
            {timesheetServiceOrderNumber},
            {timesheetJobNumber},
            {timesheetJobTaskNumber},
            {timesheetProfitCenter},
            {timesheetCreatedDate},
            {timesheetModifiedDate},
            {timesheetUpdatedBy},            
            {timesheetStatus},
            {timesheetIsTimeoff},
            {timesheetIsDeletable}
            ";

            var query = $@"IF EXISTS (SELECT * FROM {TimesheetEntryTable} WHERE {nameof(TimesheetEntry.Id)} = {timesheetId})
                         BEGIN
                             UPDATE {TimesheetEntryTable}
                             SET {updates}
                             WHERE {nameof(TimesheetEntry.Id)} = {timesheetId};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {TimesheetEntryTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            var entry = timesheet.TimesheetEntries.FirstOrDefault();
            if (entry is not null)
            {
                _databaseService.ExecuteAsync(query, new
                {
                    timesheetId = entry.Id,
                    timesheetHeaderId = timesheet.Id,
                    timesheetEmployeeId = entry.EmployeeId,
                    timesheetWorkDate = entry.WorkDate,
                    timesheetPayrollCodeId = entry.PayrollCodeId,
                    timesheetHours = entry.Hours,
                    timesheetDescription = entry.Description,
                    timesheetServiceOrderNumber = entry.ServiceOrderNumber,
                    timesheetJobNumber = entry.JobNumber,
                    timesheetJobTaskNumber = entry.JobTaskNumber,
                    timesheetProfitCenter = entry.ProfitCenterNumber,
                    timesheetCreatedDate = entry.CreatedDate,
                    timesheetModifiedDate = entry.ModifiedDate,
                    timesheetUpdatedBy = entry.UpdatedBy,
                    timesheetStatus = entry.Status,
                    timesheetIsTimeoff = false,
                    timesheetIsDeletable = false
                }).Wait();
            }
        }
    }
}
