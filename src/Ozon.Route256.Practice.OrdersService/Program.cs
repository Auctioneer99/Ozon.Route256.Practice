using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Ozon.Route256.Practice.OrdersService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(x => x
                .UseStartup<Startup>()
                .ConfigureKestrel(options =>
                {
                    var grpcPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_GRPC_PORT")!);

                    options.Listen(
                        IPAddress.Any,
                        grpcPort,
                        listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                }));
    }
}