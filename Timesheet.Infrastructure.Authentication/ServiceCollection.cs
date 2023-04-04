using Microsoft.Extensions.DependencyInjection;
using Timesheet.Infrastructure.Authentication.Models;
using Timesheet.Infrastructure.Authentication.Providers;

namespace Timesheet.Infrastructure.Authentication
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            return services.AddScoped<IAuthenticationService<AuthenticationResponse>, JwtAuthenticationService>()
            .AddScoped<IAuthenticator, ADAuthenticator>();
        }
    }
}
