﻿using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Timesheet.Infrastructure.Dapper
{
    public class DatabaseService : IDatabaseService
    {
        private string _dbConnectionString;

        public DatabaseService(ISqlConnectionString connectionString)
        {
            _dbConnectionString = connectionString.Value;
        }

        public async Task<List<T>> QueryAsync<T>(string query, object? @params = null)
        {
            var data = new List<T>();
            using var _dbConnection = new SqlConnection(_dbConnectionString);
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
            
            return data;
        }

        public List<T> Query<T>(string query, object? @params = null)
        {
            var data = new List<T>();
            using var _dbConnection = new SqlConnection(_dbConnectionString);
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
           
            return data;
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object? @params = null)
        {
            using var _dbConnection = new SqlConnection(_dbConnectionString);
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
        }

        public async Task ExecuteAsync(string query, object? @params = null)
        {
            var _dbConnection = new SqlConnection(_dbConnectionString);
            try
            {
                if (@params is null)
                {
                    await _dbConnection.ExecuteAsync(query);
                }
                else
                {
                    await _dbConnection.ExecuteAsync(query, @params);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(query + System.Environment.NewLine + ex.Message);
            }
        }

        public void Execute(string query, object? @params = null)
        {
            using var _dbConnection = new SqlConnection(_dbConnectionString);
            try
            {
                if (@params is null)
                {
                    _dbConnection.Execute(query);
                }
                else
                {
                    _dbConnection.Execute(query, @params);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(query + System.Environment.NewLine + ex.Message);
            }
        }

        public async Task ExecuteTransactionAsync(Action doTransaction)
        {
            using (var transactionScope = new TransactionScope())
            {
                doTransaction();
                transactionScope.Complete();
            }
        }
    }
}