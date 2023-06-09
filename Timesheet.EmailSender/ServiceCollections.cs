using Microsoft.Extensions.DependencyInjection;
using Timesheet.EmailSender.Repositories;
using Timesheet.EmailSender.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services, Func<IServiceProvider, ISqlConnectionString> getConnectionString, string webAppUri, string templatesBasePath)
        {
            return services.AddLogging()
                .AddSingletonDatabaseQueryService()
                .AddSingleton(typeof(ISqlConnectionString), getConnectionString)
                .AddSingleton<ISettingRepository, SettingRepository>()
                .AddSingleton<INotificationRepository, NotificationRepository>()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IEmployeeRepository, EmployeeRepository>()
                .AddSingleton(typeof(ITemplateProcessor), sp => new TemplateProcessor(webAppUri, templatesBasePath));
        }
    }
}
