using Npgsql;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Impl;

public sealed class PostgresConnectionFactory : IPostgresConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}