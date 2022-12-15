using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal partial class PayrollRepository : IRepository<TimesheetHeader>
    {
        private const string TimesheetEntryTable = "TimesheetEntry";

        public async Task UpSertEntry(TimesheetHeader timesheet)
        {
            var timesheetId = "@timesheetId";
            var timesheetHeaderId = "@timesheetHeaderId";
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
            var timesheetUpdatedBy = "@timesheetUpdatedBy";

            var updates = $@"
            {nameof(TimesheetEntry.EmployeeId)} = {timesheetEmployeeId},
            {nameof(TimesheetEntry.WorkDate)} = {timesheetWorkDate},
            {nameof(TimesheetEntry.PayrollCodeId)} = {timesheetPayrollCode},
            {nameof(TimesheetEntry.Hours)} = {timesheetHours},
            {nameof(TimesheetEntry.Description)} = {timesheetDescription},
            {nameof(TimesheetEntry.ServiceOrderNumber)} = {timesheetServiceOrderNumber},
            {nameof(TimesheetEntry.JobNumber)} = {timesheetJobNumber},
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
            {nameof(TimesheetEntry.ProfitCenterNumber)},
            {nameof(TimesheetEntry.CreatedDate)},
            {nameof(TimesheetEntry.ModifiedDate)},
            {nameof(TimesheetEntry.UpdatedBy)}
            ";

            var insertValues = $@"
            {timesheetId},
            {timesheetHeaderId},
            {timesheetEmployeeId},
            {timesheetWorkDate},
            {timesheetPayrollCode},
            {timesheetHours},
            {timesheetDescription},
            {timesheetServiceOrderNumber},
            {timesheetJobNumber},
            {timesheetProfitCenter},
            {timesheetCreatedDate},
            {timesheetModifiedDate},
            {timesheetUpdatedBy}
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
                    timesheetPayrollCode = entry.PayrollCodeId,
                    timesheetHours = entry.Hours,
                    timesheetDescription = entry.Description,
                    timesheetServiceOrderNumber = entry.ServiceOrderNumber,
                    timesheetJobNumber = entry.JobNumber,
                    timesheetProfitCenter = entry.ProfitCenterNumber,
                    timesheetCreatedDate = entry.CreatedDate,
                    timesheetModifiedDate = entry.ModifiedDate,
                    timesheetUpdatedBy = entry.UpdatedBy
                }).Wait();
            }
        }
    }
}
