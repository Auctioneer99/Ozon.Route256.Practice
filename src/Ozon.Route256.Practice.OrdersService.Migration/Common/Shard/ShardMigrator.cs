using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Interfaces;

namespace Ozon.Route256.Practice.OrdersService.Migration.Common.Shard;

internal sealed class ShardMigrator : IShardMigrator
{
    private readonly IServiceDiscoveryRepository _discoveryRepository;
    private readonly DbOptions _options;

    public ShardMigrator(IServiceDiscoveryRepository discoveryRepository, IOptions<DbOptions> options)
    {
        _discoveryRepository = discoveryRepository;
        _options = options.Value;
    }

    public async Task Migrate(CancellationToken token)
    {
        var endpoints = await _discoveryRepository.GetEndpoints(token);

        foreach (var endpoint in endpoints)
        {
            var connectionString = GetConnectionString(endpoint);

            var serviceProvider = CreateService(connectionString);
            
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ShardMigrationContext>();
            context.SetSchema(endpoint.Bucket);
                
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
    
    private string GetConnectionString(DbEndpoint endpoint)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host     = endpoint.HostAndPort,
            Database = _options.DatabaseName,
            Username = _options.User,
            Password = _options.Password
        };
        return builder.ToString();
    }

    private IServiceProvider CreateService(string connectionString)
    {
        var services = new ServiceCollection()
            .AddSingleton<ShardMigrationContext>()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder =>
                builder
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(SqlMigration).Assembly).For.Migrations()
                    .ScanIn(typeof(ShardVersionTableMetaData).Assembly).For.VersionTableMetaData());
            
        services.AddOptions<ProcessorOptions>()
            .Configure(options =>
            {
                options.ConnectionString = connectionString;
                options.Timeout = TimeSpan.FromSeconds(10);
            });

        return services
            .BuildServiceProvider();
    }
}