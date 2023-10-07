using Grpc.AspNetCore.Server;

namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddOrdersValidator(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetCustomerOrdersRequest>, GetCustomerOrdersRequestValidator>();
        services.AddTransient<IValidator<GetOrdersAggregationRequest>, GetOrdersAggregationRequestValidator>();
        services.AddTransient<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>();
        services.AddTransient<IValidator<GetStatusByIdRequest>, GetStatusByIdRequestValidator>();
        services.AddTransient<IValidator<CancelRequest>, CancelRequestValidator>();

        return services;
    }
}