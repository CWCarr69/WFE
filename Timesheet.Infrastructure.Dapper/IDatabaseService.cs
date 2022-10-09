namespace Timesheet.Infrastructure.Dapper
{
    public interface IDatabaseService
    {
        public Task<List<T>> QueryAsync<T>(string query, object? @params=null);
        public List<T> Query<T>(string query, object? @params=null);
        public void Execute(string query, object? @params=null);
        public Task<T> ExecuteScalarAsync<T>(string query, object? @params=null);
    }
}
