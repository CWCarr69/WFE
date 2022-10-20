using Microsoft.Extensions.DependencyInjection;
using Timesheet.Infrastructure.Authentication.Providers;

namespace Timesheet.Infrastructure.Authentication
{
    public static class ServiceCollection
    {
        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService<string>, JwtAuthenticationService>();
            services.AddScoped<IAuthenticator, ADAuthenticator>();
        }
    }
}
