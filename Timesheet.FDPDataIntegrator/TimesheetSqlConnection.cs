using System.Data.Common;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator
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
