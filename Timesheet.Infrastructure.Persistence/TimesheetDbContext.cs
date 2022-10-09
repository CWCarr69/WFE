using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Infrastructure.Persistence
{
    internal class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : base(options)
        {
        }

        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimesheetHeader> Timesheets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationItem> NotificationItems { get; set; }
        public DbSet<Audit> Audits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var employee = builder.Entity<Employee>();

            employee.OwnsOne(e => e.EmploymentData);
            employee.OwnsOne(e => e.Contacts);
            employee.HasOne(e => e.PrimaryApprover);
            employee.HasOne(e => e.SecondaryApprover);
            //employee.OwnsOne(e => e.Benefits);
        }
    }
}
