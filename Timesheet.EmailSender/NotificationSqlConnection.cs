using System.Data.Common;
using System.Data.SqlClient;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender
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
