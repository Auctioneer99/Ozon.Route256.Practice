using Ozon.Route256.Practice.OrdersService.Extensions;
using Ozon.Route256.Practice.OrdersService.GrpcServices;

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
        });
    }
}
