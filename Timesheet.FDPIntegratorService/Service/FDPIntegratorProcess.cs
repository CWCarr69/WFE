
using Timesheet.FDPDataIntegrator;
using Timesheet.FDPDataIntegrator.Employees;
using Timesheet.FDPDataIntegrator.Payrolls;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPIntegratorService.Service
{
    internal class FDPIntegratorProcess : IFDPIntegratorProcess
    {
        private readonly IFieldPointClient _client;
        private readonly INodeReader _reader;
        private readonly IPayrollRecordProcessor _payrollProcessor;
        private readonly IEmployeeRecordProcessor _employeeProcessor;
        private readonly ILogger<FDPIntegratorProcess> _logger;

        public FDPIntegratorProcess(
            IFieldPointClient client,
            INodeReader reader,
            IPayrollRecordProcessor payrollProcessor,
            IEmployeeRecordProcessor employeeProcessor,
            ILogger<FDPIntegratorProcess> logger
            )
        {
            this._client = client;
            this._reader = reader;
            this._payrollProcessor = payrollProcessor;
            this._employeeProcessor = employeeProcessor;
            this._logger = logger;
        }

        public async Task ProcessPayrolls()
        {
            _logger.LogInformation("Payroll data processing started");
            await _client.LoadDataAsync(IntegrationType.PAYROLL);

            var response = _client.Response;
            if (response is null)
            {
                _logger.LogWarning("No response from FieldPoint while retrieving Payrolls");
                return;
            }

            var payrollRecords = _reader.Read<PayrollRecords>(_client.Response)?.Records;
            if (payrollRecords is null)
            {
                _logger.LogWarning("No data retrieved from FieldPoint while retrieving Payrolls");
                return;
            }

            await _payrollProcessor.Process(payrollRecords);
            _logger.LogInformation("Payroll data processing stopped");

        }

        public async Task ProcessEmployees()
        {
            _logger.LogInformation("Employees data processing started");
            await _client.LoadDataAsync(IntegrationType.EMPLOYEE);

            var response = _client.Response;
            if (response is null)
            {
                _logger.LogWarning("No data retrieved from FieldPoint while retrieving Employees");
                return;
            }

            var employeeRecords = _reader.Read<EmployeeRecords>(_client.Response).Records;
            if (employeeRecords is null)
            {
                _logger.LogWarning("No data retrieved from FieldPoint while retrieving Employees");
                return;
            }

            await _employeeProcessor.Process(employeeRecords);
            _logger.LogInformation("Employees data processing stopped");
        }
    }
}
