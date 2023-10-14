using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Kafka;
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
        services.AddKafka();
        
        services.AddGrpcServices(_configuration);
        services.AddGrpcClients(_configuration);

        services.AddRepositories(_configuration);
        
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
