using Microsoft.Extensions.DependencyInjection;
using Timesheet.FDPDataIntegrator.Employees;
using Timesheet.FDPDataIntegrator.Payrolls;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider();

            (var employeeProcessor, var payrollProcessor) = GetRecordsProcessors(serviceProvider);
            var nodeReader = GetNodeReader(serviceProvider);

            var settings = new FDPSettings();
            var fdpClient = new FieldPointClient(settings);
            await ProcessEmployees(employeeProcessor, nodeReader, fdpClient);
            //await ProcessPayrolls(payrollProcessor, nodeReader, fdpClient);
        }

        private static async Task ProcessPayrolls(IPayrollRecordProcessor payrollProcessor, INodeReader nodeReader, FieldPointClient client)
        {
            await client.LoadDataAsync(IntegrationType.PAYROLL);
            var payrollRecords = nodeReader.Read<PayrollRecords>(client.Response)?.Records;
            await payrollProcessor.Process(payrollRecords);
        }

        private static async Task ProcessEmployees(IEmployeeRecordProcessor employeeProcessor, INodeReader nodeReader, FieldPointClient client)
        {
            await client.LoadDataAsync(IntegrationType.EMPLOYEE);
            var employeeRecords = nodeReader.Read<EmployeeRecords>(client.Response).Records;
            await employeeProcessor.Process(employeeRecords);
        }

        private static (IEmployeeRecordProcessor employeeProcessor, IPayrollRecordProcessor payrollProcessor) 
            GetRecordsProcessors(ServiceProvider serviceProvider)
        {
            var employeeRecordProcessor = serviceProvider.GetRequiredService<IEmployeeRecordProcessor>();
            //var payrollRecordProcessor = serviceProvider.GetRequiredService<IPayrollRecordProcessor>();
            IPayrollRecordProcessor payrollRecordProcessor = null;

            return (employeeRecordProcessor, payrollRecordProcessor);
        }

        private static INodeReader GetNodeReader(ServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<INodeReader>();
        }
    }
}