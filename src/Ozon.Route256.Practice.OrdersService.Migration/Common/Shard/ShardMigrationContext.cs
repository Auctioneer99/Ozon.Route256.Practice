using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;

namespace Ozon.Route256.Practice.OrdersService.Migration.Common.Shard;

internal sealed class ShardMigrationContext
{
    public string Schema
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_schema))
            {
                throw new InvalidOperationException();
            }

            return _schema;
        }
    }
    
    private string? _schema;

    public void SetSchema(int bucket)
    {
        _schema = Shards.GetSchemaName(bucket);
    }
}