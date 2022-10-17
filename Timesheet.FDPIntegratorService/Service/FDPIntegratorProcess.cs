
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

        public FDPIntegratorProcess(
            IFieldPointClient client,
            INodeReader reader,
            IPayrollRecordProcessor payrollProcessor,
            IEmployeeRecordProcessor employeeProcessor
            )
        {
            this._client = client;
            this._reader = reader;
            this._payrollProcessor = payrollProcessor;
            this._employeeProcessor = employeeProcessor;
        }

        public async Task ProcessPayrolls()
        {
            await _client.LoadDataAsync(IntegrationType.PAYROLL);

            var response = _client.Response;
            if (response is null)
            {
                return;
            }

            var payrollRecords = _reader.Read<PayrollRecords>(_client.Response)?.Records;
            if (payrollRecords is null)
            {
                return;
            }

            await _payrollProcessor.Process(payrollRecords);
        }

        public async Task ProcessEmployees()
        {
            await _client.LoadDataAsync(IntegrationType.EMPLOYEE);

            var response = _client.Response;
            if (response is null)
            {
                return;
            }

            var employeeRecords = _reader.Read<EmployeeRecords>(_client.Response).Records;
            if (employeeRecords is null)
            {
                return;
            }

            await _employeeProcessor.Process(employeeRecords);
        }
    }
}
