using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Employees
{
    internal class EmployeeRepository : IRepository<Employee>
    {
        private const string EmployeeTableName = "Employees";
        private readonly IDatabaseService _databaseService;

        public EmployeeRepository(IDatabaseService databaseService)
        {
            this._databaseService = databaseService;
        }

        public async Task BeginTransaction(Action transaction)
        {
            await _databaseService.ExecuteTransactionAsync(transaction);
        }

        public async Task DisableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = $"ALTER TABLE Employees NOCHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query/*, new { tableName = EmployeeTableName }*/);
        }

        public async Task EnableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = "ALTER TABLE {employeeTableParam} WITH CHECK CHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query, new { tableName = EmployeeTableName });
        }

        public Task UpSert(Employee employee)
        {
            throw new NotImplementedException();
        }

        private Task Add(Employee employee)
        {
            throw new NotImplementedException();
        }

        private Task Update(Employee employee)
        {
            throw new NotImplementedException();
        }
    }
}
