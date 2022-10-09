using Dapper;
using System.Data;

namespace Timesheet.Infrastructure.Dapper
{
    public class DatabaseService : IDatabaseService
    {
        private IDbConnection _dbConnection;

        public DatabaseService(ISqlConnection dbConnection)
        {
            _dbConnection = dbConnection.Connection;
        }

        public async Task<List<T>> QueryAsync<T>(string query, object? @params = null)
        {
            var data = new List<T>();
            _dbConnection.Open();
            try
            {
                if(@params is null)
                {
                    data = (await _dbConnection.QueryAsync<T>(query)).AsList();
                }
                else
                {
                    data = (await _dbConnection.QueryAsync<T>(query, @params)).AsList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(query + System.Environment.NewLine + ex.Message);
            }
            finally
            {
                _dbConnection.Close();
            }
            return data;
        }

        public List<T> Query<T>(string query, object? @params = null)
        {
            var data = new List<T>();
            _dbConnection.Open();
            try
            {
                if (@params is null)
                {
                    data = (_dbConnection.Query<T>(query)).AsList();
                }
                else
                {
                    data = (_dbConnection.Query<T>(query, @params)).AsList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(query + System.Environment.NewLine + ex.Message);
            }
            finally
            {
                _dbConnection.Close();
            }
            return data;
        }

        public void Execute(string query, object? @params = null)
        {
            _dbConnection.Execute(query, @params);
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object? @params = null)
        {
            _dbConnection.Open();
            try
            {
                if (@params is null)
                {
                    return await _dbConnection.ExecuteScalarAsync<T>(query);
                }
                else
                {
                    return await _dbConnection.ExecuteScalarAsync<T>(query, @params);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(query + System.Environment.NewLine + ex.Message);
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}