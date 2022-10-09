using System.Data.Common;

namespace Timesheet.Infrastructure.Dapper
{
    public interface ISqlConnection
    {
        public DbConnection Connection { get; }
    }
}
