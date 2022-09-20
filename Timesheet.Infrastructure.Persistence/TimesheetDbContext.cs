using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models;

namespace Timesheet.Infrastructure.Persistence
{
    internal class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : base(options)
        {
        }

        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
