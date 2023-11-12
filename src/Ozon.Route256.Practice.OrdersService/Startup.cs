using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ozon.Route256.Practice.OrdersService.Application.Services.Impl;
using Ozon.Route256.Practice.OrdersService.Extensions;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Metrics;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Tracing;
using Prometheus;
using Serilog;

namespace Ozon.Route256.Practice.OrdersService;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Debug
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMemoryUsage()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();

        services.AddSingleton<GrpcMetrics>();
        services.AddSingleton<IOrderActivitySource, OrderActivitySource>();
        
        services
            .AddSerilog()
            .AddOpenTelemetry()
            .WithTracing(x =>
            {
                x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(nameof(OrderService)));
                x.AddAspNetCoreInstrumentation();
                x.AddNpgsql();
                x.AddSource(OrderActivitySource.ActivityName);
                x.AddConsoleExporter();
                x.AddOtlpExporter();
            });
        
        // Host
        services.AddGrpcServer(_configuration);
        services.AddEndpointsApiExplorer();

        //Infrastructure
        services.AddKafka();
        services.AddPostgres(_configuration);
        
        //Grpc clients
        services.AddGrpcClients(_configuration);
        
        //Application
        services.AddRepositories(_configuration);
        services.AddServices();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(x =>
        {
            x.MapGrpcService<OrdersGrpcService>();
            x.MapGrpcReflectionService();
            x.MapMetrics();
        });
    }
}
