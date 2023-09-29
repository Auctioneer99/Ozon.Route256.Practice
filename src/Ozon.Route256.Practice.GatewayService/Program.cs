using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.GatewayService;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(x => x.UseStartup<Startup>().ConfigureKestrel(options =>
    {
        var httpPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_HTTP_PORT")!);
        options.Listen(
            IPAddress.Any, 
            httpPort,
            listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
    }))
    .Build()
    .RunAsync();