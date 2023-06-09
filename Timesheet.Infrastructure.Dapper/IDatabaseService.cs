namespace Timesheet.Infrastructure.Dapper
{
    public interface IDatabaseService
    {
        Task<List<T>> QueryAsync<T>(string query, object? @params=null);
        List<T> Query<T>(string query, object? @params=null);
        Task ExecuteAsync(string query, object? @params=null);
        void Execute(string query, object? @params=null);
        Task<T> ExecuteScalarAsync<T>(string query, object? @params=null);
        Task ExecuteTransactionAsync(Action transaction);
        void ExecuteTransaction(Action transaction);
    }
}
