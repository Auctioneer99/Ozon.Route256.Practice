using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Ozon.Route256.Practice.GatewayService.Controllers;
using Ozon.Route256.Practice.OrdersService;

namespace Ozon.Route256.Practice.GatewayService;

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
        var balancerAddresses = _configuration.GetValue<string>("ROUTE256_ORDER_SERVICE_ADDRESSES")
            .Split(";")
            .Select(x => x.Split(":"))
            .Select(x => new BalancerAddress(x[0], int.Parse(x[1])))
            .ToArray();
        var customerAddressRaw = _configuration.GetValue<string>("ROUTE256_CUSTOMER_SERVICE_ADDRESS")
            .Split(":");
        var customerAddress = new List<BalancerAddress>() { new BalancerAddress(customerAddressRaw[0], int.Parse(customerAddressRaw[1])) };
        
        var factory = new StaticResolverFactory(address =>
            address.Host switch
            {
                "order-service" => balancerAddresses,
                "customer-service" => customerAddress,
                _ => Array.Empty<BalancerAddress>()
            }
        );
        services.AddSingleton<ResolverFactory>(factory);

        services
            .AddGrpcClient<Orders.OrdersClient>(options => options.Address = new Uri("static://order-service"))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
                x.ServiceConfig = new ServiceConfig()
                {
                    LoadBalancingConfigs = { new LoadBalancingConfig("round_robin") }
                };
            });

        services.AddGrpcClient<Customers.Customers.CustomersClient>(options =>
            options.Address = new Uri("static://customer-service"))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
            });
        
        
        services.AddTransient<CustomerController>();
        services.AddTransient<OrderController>();

        services.AddAutoMapper(typeof(Program));
        
        services.AddSwaggerGen();
        services.AddSwaggerGenNewtonsoftSupport();
        
        services.AddControllers().AddNewtonsoftJson();
        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseEndpoints(x =>
        {
            x.MapControllers();
        });
    }
}
