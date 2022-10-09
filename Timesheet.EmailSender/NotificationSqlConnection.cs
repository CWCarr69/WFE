using System.Data.Common;
using System.Data.SqlClient;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender
{
    internal class NotificationSqlConnection : ISqlConnection
    {
        public DbConnection Connection { get; private set; }
        public NotificationSqlConnection(string connectionString= "")
        {
            this.Connection = new SqlConnection(connectionString);
        }
    }
}
