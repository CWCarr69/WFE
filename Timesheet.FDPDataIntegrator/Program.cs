using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Timesheet.FDPDataIntegrator.Employees;
using Timesheet.FDPDataIntegrator.Payrolls;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            /*var serviceProvider = new ServiceCollection().AddServices(ConfigurationManager.ConnectionStrings["Timesheet"].ToString()).BuildServiceProvider();

            (var employeeProcessor, var payrollProcessor) = GetRecordsProcessors(serviceProvider);
            var nodeReader = GetNodeReader(serviceProvider);
            var settingsRepository = GetSettings(serviceProvider);

            var settings = new FDPSettings();
            var fdpClient = new FieldPointClient(settingsRepository, null);
            await ProcessEmployees(employeeProcessor, nodeReader, fdpClient);*/
            //await ProcessPayrolls(payrollProcessor, nodeReader, fdpClient);
        }

        /*private static async Task ProcessPayrolls(IPayrollRecordProcessor payrollProcessor, INodeReader nodeReader, FieldPointClient client)
        {
            await client.LoadDataAsync(IntegrationType.PAYROLL);

            var response = client.Response;
            if (response is null)
            {
                await Task.CompletedTask;
            }

            var payrollRecords = nodeReader.Read<PayrollRecords>(client.Response)?.Records;
            if (payrollRecords is null)
            {
                await Task.CompletedTask;
            }

            await payrollProcessor.Process(payrollRecords);
        }

        private static async Task ProcessEmployees(IEmployeeRecordProcessor employeeProcessor, INodeReader nodeReader, FieldPointClient client)
        {
            await client.LoadDataAsync(IntegrationType.EMPLOYEE);

            var response = client.Response;
            if (response is null)
            {
                await Task.CompletedTask;
            }

            var employeeRecords = nodeReader.Read<EmployeeRecords>(client.Response).Records;
            if (employeeRecords is null)
            {
                await Task.CompletedTask;
            }

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


        private static ISettingRepository GetSettings(ServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<ISettingRepository>();
        }*/
    }
}