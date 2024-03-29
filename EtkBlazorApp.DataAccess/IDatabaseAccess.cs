﻿using Dapper;
using Dapper.Mapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess;

public interface IDatabaseAccess
{
    Task<List<T>> GetList<T, U>(string sql, U parameters);
    Task<List<T>> GetList<T>(string sql);
    Task<List<TMain>> GetListWithChild<TMain, TSub1, U>(string sql, string splitColumnName, U parameters);


    Task<TMain> GetSingleWithChild<TMain, TSub1, U>(string sql, string splitColumnName, U parameters);
    Task<T> GetFirstOrDefault<T, U>(string sql, U parameters);
    Task<T> GetFirstOrDefault<T>(string sql);
    Task<T> GetScalar<T, U>(string sql, U parameters);
    Task<T> GetScalar<T>(string sql);

    Task ExecuteQuery<T>(string sql, T parameters);
    Task ExecuteQuery(string sql);


}

public class EtkDatabaseDapperAccess : IDatabaseAccess
{
    private readonly IConfiguration configuration;

    string ConnectionString => configuration.GetConnectionString("etk_db");

    public EtkDatabaseDapperAccess(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<List<T>> GetList<T>(string sql)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            var rows = await connection.QueryAsync<T>(sql);

            return rows.ToList();
        }
    }

    public async Task<List<T>> GetList<T, U>(string sql, U parameters)
    {
        using var connection = new MySqlConnection(ConnectionString);

        var rows = await connection.QueryAsync<T>(sql, parameters);

        return rows.ToList();

    }

    public async Task<List<TMain>> GetListWithChild<TMain, TSub1, U>(string sql, string splitColumnName, U parameters)
    {
        using var connection = new MySqlConnection(ConnectionString);

        var rows = await connection.QueryAsync<TMain, TSub1>(sql, parameters, splitOn: splitColumnName);

        return rows.ToList();
    }

    public async Task<TMain> GetSingleWithChild<TMain, TSub1, U>(string sql, string splitColumnName, U parameters)
    {
        return (await GetListWithChild<TMain, TSub1, U>(sql, splitColumnName, parameters)).FirstOrDefault();
    }

    public async Task<T> GetFirstOrDefault<T, U>(string sql, U parameters)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            var item = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);

            return item;
        }
    }

    public async Task<T> GetFirstOrDefault<T>(string sql)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            T value = await connection.QueryFirstOrDefaultAsync<T>(sql);

            return value;
        }
    }

    public async Task<T> GetScalar<T, U>(string sql, U parameters)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            T value = await connection.ExecuteScalarAsync<T>(sql, parameters);

            return value;
        }
    }

    public async Task<T> GetScalar<T>(string sql)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            T value = await connection.ExecuteScalarAsync<T>(sql);

            return value;
        }
    }

    public async Task ExecuteQuery<T>(string sql, T parameters)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task ExecuteQuery(string sql)
    {
        using (IDbConnection connection = new MySqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(sql);
        }
    }


}
