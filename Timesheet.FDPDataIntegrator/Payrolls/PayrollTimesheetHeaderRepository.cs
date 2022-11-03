using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal partial class PayrollRepository : IRepository<TimesheetHeader>
    {
        private const string TimesheetTable = "Timesheets";
        private IDatabaseService _databaseService;

        public PayrollRepository(IDatabaseService databaseService)
        {
            this._databaseService = databaseService;
        }

        public async Task BeginTransaction(Action transaction)
        {
            await _databaseService.ExecuteTransactionAsync(transaction);
        }

        public async Task UpSert(TimesheetHeader timesheet)
        {
            var timesheetId = "@timesheetId";
            var timesheetPayrollPeriod = "@timesheetPayrollPeriod";
            var timesheetStartDate = "@timesheetStartDate";
            var timesheetEndDate = "@timesheetEndDate";
            var timesheetStatus = "@timesheetStatus";
            var timesheetCreatedDate = "@timesheetCreatedDate";
            var timesheetModifiedDate = "@timesheetModifiedDate";
            var timesheetUpdatedBy = "@timesheetUpdatedBy";

            var updates = $@"
            {nameof(TimesheetHeader.PayrollPeriod)} = {timesheetPayrollPeriod},
            {nameof(TimesheetHeader.StartDate)} = {timesheetStartDate},
            {nameof(TimesheetHeader.EndDate)} = {timesheetEndDate},
            {nameof(TimesheetHeader.ModifiedDate)} = {timesheetModifiedDate},
            {nameof(TimesheetHeader.UpdatedBy)} = {timesheetUpdatedBy}
            ";

            var insertColums = $@"
            {nameof(TimesheetHeader.Id)},
            {nameof(TimesheetHeader.PayrollPeriod)},
            {nameof(TimesheetHeader.StartDate)},
            {nameof(TimesheetHeader.EndDate)},
            {nameof(TimesheetHeader.Status)},
            {nameof(TimesheetHeader.CreatedDate)},
            {nameof(TimesheetHeader.ModifiedDate)},
            {nameof(TimesheetHeader.UpdatedBy)}
            ";

            var insertValues = $@"
            {timesheetId},
            {timesheetPayrollPeriod},
            {timesheetStartDate},
            {timesheetEndDate},
            {timesheetStatus},
            {timesheetCreatedDate},
            {timesheetModifiedDate},
            {timesheetUpdatedBy}
            ";


            var query = $@"IF EXISTS (SELECT * FROM {TimesheetTable} WHERE {nameof(TimesheetHeader.PayrollPeriod)} = {timesheetPayrollPeriod})
                         BEGIN
                             UPDATE {TimesheetTable}
                             SET {updates}
                             WHERE {nameof(TimesheetHeader.PayrollPeriod)} = {timesheetPayrollPeriod};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {TimesheetTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            _databaseService.ExecuteAsync(query, new
            {
                timesheetId = timesheet.Id,
                timesheetPayrollPeriod = timesheet.PayrollPeriod,
                timesheetStartDate = timesheet.StartDate,
                timesheetEndDate = timesheet.EndDate,
                timesheetStatus = timesheet.Status,
                timesheetCreatedDate = timesheet.CreatedDate,
                timesheetModifiedDate = timesheet.ModifiedDate,
                timesheetUpdatedBy = timesheet.UpdatedBy
            }).Wait();
            
            UpSertEntry(timesheet).Wait();
        }

        
        public Task DisableConstraints()
        {
            return Task.CompletedTask;
        }

        public Task EnableConstraints()
        {
            return Task.CompletedTask;
        }
    }
}
