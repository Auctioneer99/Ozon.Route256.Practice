using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Dal;

public static class DalExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);
        
        services.AddSingleton<IPostgresConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));

        var mapper = NpgsqlConnection.GlobalTypeMapper;
        mapper.MapEnum<OrderState>("order_state");
        mapper.MapEnum<OrderType>("order_type");
        mapper.MapComposite<OrderDto>("order");
        mapper.MapComposite<AddressDto>("address");
        mapper.MapComposite<RegionDto>("region");
        
        return services;
    }
    
    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(builder =>
                builder
                    .AddPostgres()
                    .ScanIn(typeof(SqlMigration).Assembly)
                    .For.Migrations())
            .AddOptions<ProcessorOptions>()
            .Configure(options =>
            {
                options.ConnectionString = connectionString;
                options.Timeout = TimeSpan.FromSeconds(5);
            });
        
        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetValue<string>("Postgres:ConnectionString");
    }
}