using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Employees.EventHandlers;
using Timesheet.Application.Holidays.CommandHandlers;
using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application
{
    public static class ServiceCollection
    {
        public static void RegisterEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddSingleton<HandlersConfiguration, HandlersConfiguration>();
        }

        public static void RegisterEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<HolidayAddedHandler, HolidayAddedHandler>();
            services.AddScoped<HolidayDeletedHandler, HolidayDeletedHandler>();
            services.AddScoped<HolidayGeneralInformationsUpdatedHandler, HolidayGeneralInformationsUpdatedHandler>();

        }

        public static void RegisterCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<AddHolidayCommandHandler, AddHolidayCommandHandler>();
            services.AddScoped<DeleteHolidayCommandHandler, DeleteHolidayCommandHandler>();
            services.AddScoped<SetHolidayAsRecurrentCommandHandler, SetHolidayAsRecurrentCommandHandler>();
            services.AddScoped<UpdateHolidayGeneralInformationsCommandHandler, UpdateHolidayGeneralInformationsCommandHandler>();
        }
    }
}
