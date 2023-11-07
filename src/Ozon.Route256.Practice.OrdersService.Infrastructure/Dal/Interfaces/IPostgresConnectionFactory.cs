using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}