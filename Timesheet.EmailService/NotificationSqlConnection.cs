using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailService
{
    internal class NotificationSqlConnection : ISqlConnectionString
    {
        public string Value { get; private set; }
        public NotificationSqlConnection(string connectionString= "")
        {
            this.Value = connectionString;
        }
    }
}
