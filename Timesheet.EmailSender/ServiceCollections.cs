using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Timesheet.EmailSender.Repositories;
using Timesheet.EmailSender.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender
{
    internal static class ServiceCollections
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddLogging()
                .AddSingleton(typeof(ISqlConnectionString), sp => new NotificationSqlConnection(ConfigurationManager.ConnectionStrings["Notification"].ToString()))
                .AddSingleton<ISettingRepository, SettingRepository>()
                .AddSingleton<INotificationRepository, NotificationRepository>()
                .AddSingleton<INotificationService, NotificationService>();
        }
    }
}
