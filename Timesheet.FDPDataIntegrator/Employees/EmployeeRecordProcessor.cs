using Microsoft.Extensions.Logging;
using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Employees
{
    public interface IEmployeeRecordProcessor : IRecordProcessor<IAdapter<EmployeeRecord, Employee>, IRepository<Employee>, EmployeeRecord, Employee>
    {
        Task DeactivateMissingEmployees(EmployeeRecord[] records);
    }

    internal class EmployeeRecordProcessor
        : RecordProcessor<IAdapter<EmployeeRecord, Employee>, IRepository<Employee>, EmployeeRecord, Employee>, IEmployeeRecordProcessor
    {
        private readonly IRepository<Employee> _repository;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<EmployeeRecordProcessor> _logger;

        public EmployeeRecordProcessor(
            IRepository<Employee> repository, 
            IAdapter<EmployeeRecord, Employee> adapter, 
            IDatabaseService databaseService,
            ILogger<EmployeeRecordProcessor> logger)
            : base(repository, adapter, logger)
        {
            this._repository = repository;
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public override async Task Process(EmployeeRecord[] records)
        {
            _repository.DisableConstraints().Wait();
            var _records = records.OrderBy(r => r.ManagerId).ToArray();
            base.Process(_records).Wait();
            
            // Deactivate employees not in XML
            await DeactivateMissingEmployees(_records);
            
            //_repository.EnableConstraints().Wait();
        }

        public async Task DeactivateMissingEmployees(EmployeeRecord[] records)
        {
            try
            {
                _logger.LogInformation("Checking for employees in database but not in XML...");

                // Get all employee IDs from the XML
                var xmlEmployeeIds = records.Select(r => r.EmployeeCode).ToList();

                // Build parameters for SQL query
                var employeeIdsParam = string.Join(",", xmlEmployeeIds.Select((_, i) => $"@id{i}"));
                
                var parameters = new Dictionary<string, object>();
                for (int i = 0; i < xmlEmployeeIds.Count; i++)
                {
                    parameters[$"id{i}"] = xmlEmployeeIds[i];
                }

                // Query to find employees in database but not in XML
                var query = $@"
                    SELECT Id, FullName, IsActive, UsesTimesheet
                    FROM Employees
                    WHERE Id NOT IN ({employeeIdsParam})
                    AND (IsActive = 1 OR UsesTimesheet = 1)";

                var missingEmployees = await _databaseService.QueryAsync<dynamic>(query, parameters);

                if (missingEmployees != null && missingEmployees.Any())
                {
                    _logger.LogInformation($"Found {missingEmployees.Count()} employees in database not present in XML");

                    // Update each missing employee
                    foreach (var employee in missingEmployees)
                    {
                        var updateQuery = @"
                            UPDATE Employees
                            SET IsActive = 0,
                                UsesTimesheet = 0,
                                ModifiedDate = @modifiedDate
                            WHERE Id = @employeeId";

                        await _databaseService.ExecuteAsync(updateQuery, new
                        {
                            employeeId = employee.Id,
                            modifiedDate = DateTime.Now
                        });

                        _logger.LogInformation($"Deactivated employee {employee.FullName} (ID: {employee.Id}) - not found in XML");
                    }
                }
                else
                {
                    _logger.LogInformation("No employees need to be deactivated");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating missing employees: {ex.Message}");
                throw;
            }
        }
    }
}
