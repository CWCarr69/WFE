using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;
using Timesheet.Infrastructure.Persistence.Queries;
using Timesheet.Infrastructure.Persistence.Writes;
using Timesheet.ReadModel.Queries;

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


            services.AddScoped<IReadRepository<Employee>, ReadRepository<Employee>>();
            services.AddScoped<IHolidayReadRepository, HolidayReadRepository>();

            services.AddScoped<IWriteRepository<Holiday>, WriteRepository<Holiday>>();
            services.AddScoped<IWriteRepository<Employee>, WriteRepository<Employee>>();

            services.AddScoped<IQueryHoliday, QueryHolidays>();
            services.AddScoped<IQueryEmployee, QueryEmployee>();
            services.AddScoped<IQueryTimeoff, QueryTimeoff>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
