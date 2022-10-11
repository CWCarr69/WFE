using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.FDPDataIntegrator.Employees;
using Timesheet.FDPDataIntegrator.Payrolls;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator
{
    internal static class ServiceCollections
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddLogging()
                .AddSingleton<IDatabaseService, DatabaseService>()

                .AddSingleton(typeof(ISqlConnectionString), sp => new TimesheetSqlConnection(ConfigurationManager.ConnectionStrings["Timesheet"].ToString()))

                .AddSingleton<IRepository<Employee>, EmployeeRepository>()
                .AddSingleton<IRepository<TimesheetHeader>, PayrollRepository>()

                .AddSingleton<IAdapter<PayrollRecord, TimesheetHeader>, PayrollAdapter>()
                .AddSingleton<IAdapter<EmployeeRecord, Employee>, EmployeeAdapter>()
                
                .AddSingleton<IEmployeeRecordProcessor, EmployeeRecordProcessor>()
                .AddSingleton<IPayrollRecordProcessor, PayrollRecordProcessor>()

                .AddSingleton<INodeReader, NodeReader>();

        }
    }
}
