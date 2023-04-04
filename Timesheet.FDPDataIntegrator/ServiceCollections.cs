using Microsoft.Extensions.DependencyInjection;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Employees;
using Timesheet.FDPDataIntegrator.Payrolls;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string connectionString)
        {
            return services.AddLogging()
                .AddSingletonDatabaseQueryService()
                .AddSingleton(typeof(ISqlConnectionString), sp => new TimesheetSqlConnection(connectionString))

                .AddSingleton<IRepository<Employee>, EmployeeRepository>()
                .AddSingleton<IRepository<TimesheetHeader>, PayrollRepository>()
                .AddSingleton<ISettingRepository, SettingRepository>()
                .AddSingleton<IPayrollTypesRepository, PayrollTypesRepository>()

                .AddSingleton<IAdapter<PayrollRecord, TimesheetHeader>, PayrollAdapter>()
                .AddSingleton<IAdapter<EmployeeRecord, Employee>, EmployeeAdapter>()
                
                .AddSingleton<IEmployeeRecordProcessor, EmployeeRecordProcessor>()
                .AddSingleton<IPayrollRecordProcessor, PayrollRecordProcessor>()

                .AddSingleton<INodeReader, NodeReader>()

                .AddSingleton<IFieldPointClient, FieldPointClient>();
        }
    }
}
