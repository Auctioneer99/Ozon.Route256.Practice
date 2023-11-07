using Ozon.Route256.Practice.OrdersService.Grpc.Orders;
using Ozon.Route256.Practice.OrdersService.Services.Validation;

namespace Ozon.Route256.Practice.OrdersService.Extensions;

internal static class ValidatorExtensions
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