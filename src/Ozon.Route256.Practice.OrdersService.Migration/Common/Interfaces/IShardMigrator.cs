namespace Ozon.Route256.Practice.OrdersService.Migration.Common.Interfaces;

internal interface IShardMigrator
{
    public Task Migrate(CancellationToken token);
}