using FluentMigrator.Runner.VersionTableInfo;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;

public sealed class ShardVersionTableMetaData : IVersionTableMetaData
{
    private readonly ShardMigrationContext _context;

    public object ApplicationContext { get; set; }
    
    public bool OwnsSchema => true;
    
    public string SchemaName => _context.Schema;
    
    public string TableName => "version_info";
    
    public string ColumnName => "version";
    
    public string DescriptionColumnName => "description";
    
    public string UniqueIndexName => "version_unq_idx";
    
    public string AppliedOnColumnName => "applied_on";

    public ShardVersionTableMetaData(ShardMigrationContext context)
    {
        _context = context;
    }
}