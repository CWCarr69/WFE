using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Domain.Models.Audits;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.Infrastructure.Persistence.Repositories;

namespace Timesheet.Infrastructure.Persistence
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddTimesheetContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<TimesheetDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            })
            .AddScoped<IEmployeeReadRepository, EmployeeReadRepository>()
            .AddScoped<ITimesheetReadRepository, TimesheetReadRepository>()
            .AddScoped<IHolidayReadRepository, HolidayReadRepository>()
            .AddScoped<INotificationReadRepository, NotificationReadRepository>()
            .AddScoped<IReadRepository<NotificationItem>, ReadRepository<NotificationItem>>()
            .AddScoped<IReadRepository<Setting>, ReadRepository<Setting>>()

            .AddScoped<IReadRepository<Audit>, ReadRepository<Audit>>()

            .AddScoped<IHierarchyRepository, HierarchyRepository>()

            .AddScoped<IWriteRepository<Holiday>, WriteRepository<Holiday>>()
            .AddScoped<IWriteRepository<Employee>, WriteRepository<Employee>>()
            .AddScoped<IWriteRepository<TimesheetHeader>, WriteRepository<TimesheetHeader>>()
            .AddScoped<IWriteRepository<Notification>, WriteRepository<Notification>>()
            .AddScoped<IWriteRepository<NotificationItem>, WriteRepository<NotificationItem>>()
            .AddScoped<IWriteRepository<Setting>, WriteRepository<Setting>>()
            .AddScoped<IWriteRepository<TimesheetException>, WriteRepository<TimesheetException>>()

            .AddScoped<IWriteRepository<Audit>, WriteRepository<Audit>>()

            .AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
