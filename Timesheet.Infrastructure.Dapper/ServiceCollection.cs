using Microsoft.Extensions.DependencyInjection;

namespace Timesheet.Infrastructure.Dapper
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddScopedDatabaseQueryService(this IServiceCollection services)
        {
            return services.AddScoped<IDatabaseService, DatabaseService>();
        }

        public static IServiceCollection AddSingletonDatabaseQueryService(this IServiceCollection services)
        {
            return services.AddSingleton<IDatabaseService, DatabaseService>();
        }
    }
}
