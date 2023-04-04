using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Timesheet.EmailSender.Repositories;
using Timesheet.EmailSender.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string connectionString, string webAppUri, string templatesBasePath)
        {
            return services.AddLogging()
                .AddSingletonDatabaseQueryService()
                .AddSingleton(typeof(ISqlConnectionString), sp => new NotificationSqlConnection(connectionString))
                .AddSingleton<ISettingRepository, SettingRepository>()
                .AddSingleton<INotificationRepository, NotificationRepository>()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IEmployeeRepository, EmployeeRepository>()
                .AddSingleton(typeof(ITemplateProcessor), sp => new TemplateProcessor(webAppUri, templatesBasePath));
        }
    }
}
