using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Migration.Extensions;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(builder => 
        builder
            .AddJsonFile("appsettings.json"))
    .ConfigureServices((builder, collection) =>
    {
        var configuration = builder.Configuration;

        collection
            .AddPostgres(configuration)
            .AddMigrations()
            .AddRepositories()
            .AddGrpcClients(configuration);
    })
    .Build();


using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

var scope = host.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IShardMigrator>();
await runner.Migrate(cts.Token);

