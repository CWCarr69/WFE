using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Domain.Models.Audits;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Repositories;
using Timesheet.Infrastructure.Persistence.Repositories;

namespace Timesheet.Infrastructure.Persistence
{
    public static class ServiceCollection
    {
        public static void AddTimesheetContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TimesheetDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();
            services.AddScoped<ITimesheetReadRepository, TimesheetReadRepository>();
            services.AddScoped<IHolidayReadRepository, HolidayReadRepository>();
            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<IReadRepository<NotificationItem>, ReadRepository<NotificationItem>>();
            services.AddScoped<IReadRepository<Setting>, ReadRepository<Setting>>();

            services.AddScoped<IReadRepository<Audit>, ReadRepository<Audit>>();

            services.AddScoped<IWriteRepository<Holiday>, WriteRepository<Holiday>>();
            services.AddScoped<IWriteRepository<Employee>, WriteRepository<Employee>>();
            services.AddScoped<IWriteRepository<Notification>, WriteRepository<Notification>>();
            services.AddScoped<IWriteRepository<NotificationItem>, WriteRepository<NotificationItem>>();
            services.AddScoped<IWriteRepository<Setting>, WriteRepository<Setting>>();

            services.AddScoped<IWriteRepository<Audit>, WriteRepository<Audit>>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
