using Ozon.Route256.Practice.OrdersService.Controllers;
using Ozon.Route256.Practice.OrdersService.Repository;

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
        services.AddGrpcServices(_configuration);
        services.AddGrpcClients(_configuration);

        services.AddRepositories();
        
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
