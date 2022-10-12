using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal partial class PayrollRepository : IRepository<TimesheetHeader>
    {
        private const string TimesheetEntryTableName = "TimesheetEntry";

        public async Task UpSertEntry(TimesheetHeader timesheet)
        {
            var timesheetTable = "@tableName";
            var timesheetId = "@timesheetId";
            var timesheetEmployeeId = "@timesheetEmployeeId";
            var timesheetWorkDate = "@timesheetWorkDate";
            var timesheetPayrollCode = "@timesheetPayrollCode";
            var timesheetHours = "@timesheetHours";
            var timesheetDescription = "@timesheetDescription";
            var timesheetServiceOrderNumber = "@timesheetServiceOrderNumber";
            var timesheetJobNumber = "@timesheetJobNumber";
            var timesheetProfitCenter = "@timesheetProfitCenter";
            var timesheetCreatedDate = "@timesheetCreatedDate";
            var timesheetModifiedDate = "@timesheetModifiedDate";

            var updates = $@"
            {nameof(TimesheetEntry.EmployeeId)} = {timesheetEmployeeId},
            {nameof(TimesheetEntry.WorkDate)} = {timesheetWorkDate},
            {nameof(TimesheetEntry.PayrollCode)} = {timesheetPayrollCode},
            {nameof(TimesheetEntry.Hours)} = {timesheetHours},
            {nameof(TimesheetEntry.Description)} = {timesheetDescription},
            {nameof(TimesheetEntry.ServiceOrderNumber)} = {timesheetServiceOrderNumber},
            {nameof(TimesheetEntry.JobNumber)} = {timesheetJobNumber},
            {nameof(TimesheetEntry.ProfitCenterNumber)} = {timesheetProfitCenter},
            {nameof(TimesheetEntry.ModifiedDate)} = {timesheetModifiedDate}
            ";

            var insertColums = $@"
            {nameof(TimesheetEntry.Id)},
            {nameof(TimesheetEntry.EmployeeId)},
            {nameof(TimesheetEntry.WorkDate)},
            {nameof(TimesheetEntry.PayrollCode)},
            {nameof(TimesheetEntry.Hours)},
            {nameof(TimesheetEntry.Description)},
            {nameof(TimesheetEntry.ServiceOrderNumber)},
            {nameof(TimesheetEntry.JobNumber)},
            {nameof(TimesheetEntry.ProfitCenterNumber)},
            {nameof(TimesheetEntry.CreatedDate)},
            {nameof(TimesheetEntry.ModifiedDate)}
            ";

            var insertValues = $@"
            {timesheetId},
            {timesheetEmployeeId},
            {timesheetWorkDate},
            {timesheetPayrollCode},
            {timesheetHours},
            {timesheetDescription},
            {timesheetServiceOrderNumber},
            {timesheetJobNumber},
            {timesheetProfitCenter},
            {timesheetCreatedDate},
            {timesheetModifiedDate}
            ";

            var query = $@"IF EXISTS (SELECT * FROM {timesheetTable} WHERE {nameof(TimesheetEntry.Id)} = {timesheetId})
                         BEGIN
                             UPDATE {timesheetTable}
                             SET {updates}
                             WHERE {nameof(TimesheetEntry.Id)} = {timesheetId};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {timesheetTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            var entry = timesheet.TimesheetEntries.FirstOrDefault();
            //TODO if null
            await _databaseService.ExecuteAsync(query, new
            {
                    timesheetTable = TimesheetEntryTableName,
                    timesheetId = entry.Id,
                    timesheetEmployeeId = entry.EmployeeId,
                    timesheetWorkDate = entry.WorkDate,
                    timesheetPayrollCode = entry.PayrollCode,
                    timesheetHours = entry.Hours,
                    timesheetDescription = entry.Description,
                    timesheetServiceOrderNumber = entry.ServiceOrderNumber,
                    timesheetJobNumber = entry.JobNumber,
                    timesheetProfitCenter = entry.ProfitCenterNumber,
                    timesheetCreatedDate = entry.CreatedDate,
                    timesheetModifiedDate = entry.ModifiedDate
                });
        }
    }
}
