using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Payrolls
{
    internal class PayrollRepository : IRepository<TimesheetHeader>
    {
        private IDatabaseService _databaseService;

        public PayrollRepository(IDatabaseService databaseService)
        {
            this._databaseService = databaseService;
        }

        public async Task BeginTransaction(Action transaction)
        {
            await _databaseService.ExecuteTransactionAsync(transaction);
        }

        public Task DisableConstraints()
        {
            return Task.CompletedTask;
        }

        public Task EnableConstraints()
        {
            return Task.CompletedTask;
        }

        public Task UpSert(TimesheetHeader employee)
        {
            throw new NotImplementedException();
        }

        private Task Add(TimesheetHeader employee)
        {
            throw new NotImplementedException();
        }

        private Task Update(TimesheetHeader employee)
        {
            throw new NotImplementedException();
        }
    }
}
