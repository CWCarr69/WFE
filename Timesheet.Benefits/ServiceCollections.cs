using Microsoft.Extensions.DependencyInjection;

namespace Timesheet.Benefits
{
    public static class ServiceCollections
    {
        public static IServiceCollection AddBenefitsServices(this IServiceCollection services)
        {
            return services.AddTransient<IEmployeeBenefitsService, EmployeeBenefitsService>();
        }
    }
}
