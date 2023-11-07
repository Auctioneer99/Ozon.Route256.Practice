using System.Data;
using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;

public sealed class ShardNpgsqlConnection : DbConnection
{
    public int Bucket { get; }
    
    public override string ConnectionString
    {   
        get => _npgsqlConnection.ConnectionString;
        set => _npgsqlConnection.ConnectionString = value;
    }
    
    public override string Database => _npgsqlConnection.Database;
    
    public override ConnectionState State => _npgsqlConnection.State;
    
    public override string DataSource => _npgsqlConnection.DataSource;
    
    public override string ServerVersion => _npgsqlConnection.ServerVersion;

    private NpgsqlConnection _npgsqlConnection;
    
    public ShardNpgsqlConnection(NpgsqlConnection npgsqlConnection, int bucket)
    {
        _npgsqlConnection = npgsqlConnection;
        Bucket = bucket;
    }
    
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => 
        _npgsqlConnection.BeginTransaction(isolationLevel);

    public override void ChangeDatabase(string databaseName) => _npgsqlConnection.ChangeDatabase(databaseName);

    public override void Close() => _npgsqlConnection.Close();

    public override void Open() => _npgsqlConnection.Open();

    protected override DbCommand CreateDbCommand()
    {
        var command = _npgsqlConnection.CreateCommand();
        return new ShardNpgsqlCommand(command, Bucket);
    }
    
    public override ValueTask DisposeAsync()
    {
        return _npgsqlConnection.DisposeAsync();
    }
}