using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Web.Api
{
    public class TimesheetDbContextFactory : IDesignTimeDbContextFactory<TimesheetDbContext>
    {
        public TimesheetDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TimesheetDbContext>();
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = Timesheet; Integrated Security = True; MultipleActiveResultSets = True");

            return new TimesheetDbContext(optionsBuilder.Options);
        }
    }
}
