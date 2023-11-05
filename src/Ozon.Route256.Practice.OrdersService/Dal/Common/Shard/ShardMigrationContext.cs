namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;

public class ShardMigrationContext
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
    
    private string _schema;

    public void SetSchema(int bucket)
    {
        _schema = Shards.GetSchemaName(bucket);
    }
}