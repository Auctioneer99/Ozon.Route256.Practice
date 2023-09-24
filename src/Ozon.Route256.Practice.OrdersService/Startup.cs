using Ozon.Route256.Practice.OrdersService.Controllers;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Services;

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
        services.AddTransient<RegionService>();

        services.AddTransient<OrdersController>();
        
        services.AddGrpc(x => x.Interceptors.Add<LoggerInterceptor>());
        services.AddGrpcReflection();
        
        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(x =>
        {
            x.MapGrpcService<OrdersController>();
            x.MapGrpcReflectionService();
        });
    }
}
