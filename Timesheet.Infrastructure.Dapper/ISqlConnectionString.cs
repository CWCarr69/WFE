using System.Data.Common;

namespace Timesheet.Infrastructure.Dapper
{
    public interface ISqlConnectionString
    {
        public string Value { get; }
    }
}
