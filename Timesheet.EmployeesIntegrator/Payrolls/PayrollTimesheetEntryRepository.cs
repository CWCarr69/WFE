using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal partial class PayrollRepository : IRepository<TimesheetHeader>
    {
        private const string TimesheetTableName = "TimesheetHeader";
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
            var timesheetTable = "@tableName";
            var timesheetId = "@timesheetId";
            var timesheetPayrollPeriod = "@timesheetPayrollPeriod";
            var timesheetStartDate = "@timesheetStartDate";
            var timesheetEndDate = "@timesheetEndDate";
            var timesheetStatus = "@timesheetStatus";
            var timesheetCreatedDate = "@timesheetCreatedDate";
            var timesheetModifiedDate = "@timesheetModifiedDate";

            var updates = $@"
            {nameof(TimesheetHeader.PayrollPeriod)} = {timesheetPayrollPeriod},
            {nameof(TimesheetHeader.StartDate)} = {timesheetStartDate},
            {nameof(TimesheetHeader.EndDate)} = {timesheetEndDate},
            {nameof(TimesheetHeader.ModifiedDate)} = {timesheetModifiedDate},
            ";

            var insertColums = $@"
            {nameof(TimesheetHeader.Id)},
            {nameof(TimesheetHeader.PayrollPeriod)},
            {nameof(TimesheetHeader.StartDate)},
            {nameof(TimesheetHeader.EndDate)},
            {nameof(TimesheetHeader.Status)},
            {nameof(TimesheetHeader.CreatedDate)},
            {nameof(TimesheetHeader.ModifiedDate)}
            ";

            var insertValues = $@"
            {timesheetId}
            {timesheetPayrollPeriod},
            {timesheetStartDate},
            {timesheetEndDate},
            {timesheetStatus},
            {timesheetCreatedDate},
            {timesheetModifiedDate}
            ";

            var query = $@"IF EXISTS (SELECT * FROM {timesheetTable} WHERE {nameof(TimesheetHeader.Id)} = {timesheetId})
                         BEGIN
                             UPDATE {timesheetTable}
                             SET {updates}
                             WHERE {nameof(TimesheetHeader.Id)} = {timesheetId};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {timesheetTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            await _databaseService.ExecuteAsync(query, new
            {
                timesheetTable = TimesheetTableName,
                timesheetId = timesheet.Id,
                timesheetPayrollPeriod = timesheet.PayrollPeriod,
                timesheetStartDate = timesheet.StartDate,
                timesheetEndDate = timesheet.EndDate,
                timesheetStatus = timesheet.Status,
                timesheetCreatedDate = timesheet.CreatedDate,
                timesheetModifiedDate = timesheet.ModifiedDate,
            });
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
