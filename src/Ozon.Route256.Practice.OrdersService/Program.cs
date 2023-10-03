using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.OrdersService;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(x => x
        .UseStartup<Startup>()
        .ConfigureKestrel(options =>
        {
            var grpcPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_GRPC_PORT")!);

            options.Listen(
                IPAddress.Any,
                grpcPort,
                listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        }))
    .Build()
    .RunAsync();