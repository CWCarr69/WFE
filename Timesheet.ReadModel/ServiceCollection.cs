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
        public static void AddTimesheedReadModelDatabase(this IServiceCollection services, string configuration)
        {
            services.AddScoped(typeof(ISqlConnectionString), sp => new TimesheetSqlConnection(configuration));
            services.AddScoped<IDatabaseService, DatabaseService>();

            services.AddScoped<IQueryHoliday, QueryHoliday>();
            services.AddScoped<IQueryReferential, QueryReferential>();
            services.AddScoped<IQueryEmployee, QueryEmployee>();
            services.AddScoped<IQueryTimeoff, QueryTimeoff>();
            services.AddScoped<IQueryTimesheet, QueryTimesheet>();
            services.AddScoped<IQuerySetting, QuerySetting>();
            services.AddScoped<IQueryNotification, QueryNotification>();
            services.AddScoped<IQueryAudit, QueryAudit>();
        }
    }
}
