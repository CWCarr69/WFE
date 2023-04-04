using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Audits.Queries;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Holidays.Queries;
using Timesheet.Application.Notifications.Queries;
using Timesheet.Application.Referential.Queries;
using Timesheet.Application.Settings.Queries;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastructure.Persistence.Queries;
using Timesheet.Infrastructure.ReadModel;
using Timesheet.Infrastructure.ReadModel.Queries;
using Timesheet.Infrastruture.ReadModel.Queries;

namespace Timesheet.ReadModel
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddTimesheedReadModelDatabase(this IServiceCollection services, string configuration)
        {
            return services.AddScopedDatabaseQueryService()
            .AddScoped(typeof(ISqlConnectionString), sp => new TimesheetSqlConnection(configuration))
            .AddScoped<IQueryHoliday, QueryHoliday>()
            .AddScoped<IQueryReferential, QueryReferential>()
            .AddScoped<IQueryEmployee, QueryEmployee>()
            .AddScoped<IQueryTimeoff, QueryTimeoff>()
            .AddScoped<IQueryTimesheet, QueryTimesheet>()
            .AddScoped<IQuerySetting, QuerySetting>()
            .AddScoped<IQueryNotification, QueryNotification>()
            .AddScoped<IQueryAudit, QueryAudit>();
        }
    }
}
