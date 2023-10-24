using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}