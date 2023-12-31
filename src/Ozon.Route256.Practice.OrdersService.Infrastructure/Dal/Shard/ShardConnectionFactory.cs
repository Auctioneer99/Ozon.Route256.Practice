﻿using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;

public sealed class ShardConnectionFactory : IShardConnectionFactory
{
    private readonly IShardsStore _shardsStore;
    private readonly DbOptions _dbOptions;

    public ShardConnectionFactory(IShardsStore shardsStore, IOptions<DbOptions> dbOptions)
    {
        _shardsStore = shardsStore;
        _dbOptions = dbOptions.Value;
    }

    public IEnumerable<int> GetAllBuckets()
    {
        for (var i = 0; i < _shardsStore.BucketsCount; i++)
        {
            yield return i;
        }
    }
    
    public ShardNpgsqlConnection GetConnectionByBucket(int bucket)
    {
        var endpoint = _shardsStore.GetEndpointByBucket(bucket);
        var connectionString = GetConnectionString(endpoint);
        return new ShardNpgsqlConnection(new NpgsqlConnection(connectionString), bucket);
    }

    private string GetConnectionString(DbEndpoint endpoint)
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            Host = endpoint.HostAndPort,
            Database = _dbOptions.DatabaseName,
            Username = _dbOptions.User,
            Password = _dbOptions.Password
        };
        return builder.ToString();
    }
}