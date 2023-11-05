using System.Data;
using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;

public sealed class ShardNpgsqlCommand : DbCommand
{
    public override string CommandText
    {
        get => _npgsqlCommand.CommandText;
        set
        {
            var bucketName = Shards.GetSchemaName(_bucket);
            var sql = value.Replace(
                Shards.BucketPlaceholder,
                bucketName);
            _npgsqlCommand.CommandText = sql;
        }
    }
    
    public override int CommandTimeout
    {
        get => _npgsqlCommand.CommandTimeout;
        set => _npgsqlCommand.CommandTimeout = value;
    }
    
    public override CommandType CommandType
    {
        get => _npgsqlCommand.CommandType;
        set => _npgsqlCommand.CommandType = value;
    }
    
    public override UpdateRowSource UpdatedRowSource 
    {
        get => _npgsqlCommand.UpdatedRowSource;
        set => _npgsqlCommand.UpdatedRowSource = value;
    }
    
    protected override DbConnection? DbConnection
    {
        get => _npgsqlCommand.Connection;
        set => _npgsqlCommand.Connection = value as NpgsqlConnection;
    }
    
    protected override DbParameterCollection DbParameterCollection => _npgsqlCommand.Parameters;
    
    protected override DbTransaction? DbTransaction
    {
        get => _npgsqlCommand.Transaction;
        set => _npgsqlCommand.Transaction = value as NpgsqlTransaction;
    }
    
    public override bool DesignTimeVisible
    {
        get => _npgsqlCommand.DesignTimeVisible;
        set => _npgsqlCommand.DesignTimeVisible = value;
    }
    
    private readonly NpgsqlCommand _npgsqlCommand;
    private readonly int _bucket;

    public ShardNpgsqlCommand(NpgsqlCommand npgsqlCommand, int bucket)
    {
        _npgsqlCommand = npgsqlCommand;
        _bucket = bucket;
    }

    public override void Cancel() => _npgsqlCommand.Cancel();

    public override int ExecuteNonQuery() => _npgsqlCommand.ExecuteNonQuery();

    public override object? ExecuteScalar() => _npgsqlCommand.ExecuteScalar();

    public override void Prepare() => _npgsqlCommand.Prepare();

    protected override DbParameter CreateDbParameter()  => _npgsqlCommand.CreateParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => _npgsqlCommand.ExecuteReader(behavior);

    public override ValueTask DisposeAsync()
    {
        return _npgsqlCommand.DisposeAsync();
    }
}