using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models.Audits;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Timesheets;
using Timesheet.Models.Referential;

namespace Timesheet.Infrastructure.Persistence
{
    public class TimesheetDbContext : DbContext
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
        public DbSet<Setting> Settings { get; set; }
        public DbSet<PayrollTypes> PayrollTypes { get; set; }
        public DbSet<TimesheetException> TImesheetException { get; set; }
        public DbSet<EmployeeHierarchy> EmployeeHierarchy { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            var employee = builder.Entity<Employee>();

            employee.HasMany(e => e.Timeoffs)
                .WithOne()
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

            var employmentData = employee.OwnsOne(e => e.EmploymentData);
            employmentData.Property(d => d.JobTitle).HasColumnName(nameof(EmployeeEmploymentData.JobTitle));
            employmentData.Property(d => d.Department).HasColumnName(nameof(EmployeeEmploymentData.Department));
            employmentData.Property(d => d.EmploymentDate).HasColumnName(nameof(EmployeeEmploymentData.EmploymentDate));
            employmentData.Property(d => d.TerminationDate).HasColumnName(nameof(EmployeeEmploymentData.TerminationDate));
            employmentData.Property(d => d.IsSalaried).HasColumnName(nameof(EmployeeEmploymentData.IsSalaried));
            employmentData.Property(d => d.IsAdministrator).HasColumnName(nameof(EmployeeEmploymentData.IsAdministrator));

            var contacts = employee.OwnsOne(e => e.Contacts);
            contacts.Property(d => d.CompanyEmail).HasColumnName(nameof(EmployeeContactData.CompanyEmail));
            contacts.Property(d => d.CompanyPhone).HasColumnName(nameof(EmployeeContactData.CompanyPhone));

            var employeeBenefits = employee.OwnsOne(e => e.BenefitsVariation);
            employeeBenefits.Property(d => d.VacationHours).HasColumnName(nameof(EmployeeBenefits.VacationHours));
            employeeBenefits.Property(d => d.PersonalHours).HasColumnName(nameof(EmployeeBenefits.PersonalHours));
            employeeBenefits.Property(d => d.RolloverHours).HasColumnName(nameof(EmployeeBenefits.RolloverHours));

            var employeeBenefitsSnapshot = employee.OwnsOne(e => e.BenefitsSnapshot);
            employeeBenefitsSnapshot.Property(d => d.VacationHours).HasColumnName(nameof(EmployeeBenefits.VacationHours) + "Snapshot");
            employeeBenefitsSnapshot.Property(d => d.PersonalHours).HasColumnName(nameof(EmployeeBenefits.PersonalHours) + "Snapshot");
            employeeBenefitsSnapshot.Property(d => d.RolloverHours).HasColumnName(nameof(EmployeeBenefits.RolloverHours) + "Snapshot");

            employee.HasOne(e => e.PrimaryApprover);
            employee.HasOne(e => e.SecondaryApprover);
            employee.HasOne(e => e.Manager);

            builder.Entity<TimesheetHeader>().Ignore(e => e.TimesheetEntriesWithoutTimeoffs);

            builder.Entity<EmployeeHierarchy>()
                .ToView(nameof(EmployeeHierarchy))
                .HasNoKey();
        }

    }
}
