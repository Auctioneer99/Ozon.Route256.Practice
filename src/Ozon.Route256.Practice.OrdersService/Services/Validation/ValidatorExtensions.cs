namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddOrdersValidator(this IServiceCollection services)
    {
        services.AddTransient<IValidator<Grpc.Orders.GetCustomerOrdersRequest>, GetCustomerOrdersRequestValidator>();
        services.AddTransient<IValidator<Grpc.Orders.GetOrdersAggregationRequest>, GetOrdersAggregationRequestValidator>();
        services.AddTransient<IValidator<Grpc.Orders.GetOrdersRequest>, GetOrdersRequestValidator>();
        services.AddTransient<IValidator<Grpc.Orders.GetStatusByIdRequest>, GetStatusByIdRequestValidator>();
        services.AddTransient<IValidator<Grpc.Orders.CancelRequest>, CancelRequestValidator>();

        return services;
    }
}