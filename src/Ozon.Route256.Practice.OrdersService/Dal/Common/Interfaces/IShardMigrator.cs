namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

public interface IShardMigrator
{
    public Task Migrate(CancellationToken token);
}