using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Web.Api
{
    internal class TimesheetSqlConnection : ISqlConnectionString
    {
        public string Value { get; private set; }
        public TimesheetSqlConnection(string connectionString)
        {
            this.Value = connectionString;
        }
    }
}
