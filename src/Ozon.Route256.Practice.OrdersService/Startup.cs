using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Controllers;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Services;
using Ozon.Route256.Practice.ServiceDiscovery;

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
        services.AddGrpcClient<SdService.SdServiceClient>(options =>
        {
            var url = _configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

            options.Address = new Uri(url);
        });

        services.AddSingleton<IDbStore, DbStore>();
        services.AddHostedService<SdConsumerHostedService>();

        services.AddTransient<RegionService>();
        services.AddTransient<OrdersGrpcService>();
        
        services.AddGrpc(x => x.Interceptors.Add<LoggerInterceptor>());
        services.AddGrpcReflection();
        
        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(x =>
        {
            x.MapGrpcService<OrdersGrpcService>();
            x.MapGrpcReflectionService();
        });
    }
}
