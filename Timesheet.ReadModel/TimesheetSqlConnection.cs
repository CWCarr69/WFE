using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.ReadModel
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
