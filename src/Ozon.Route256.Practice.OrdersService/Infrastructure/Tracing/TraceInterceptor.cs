using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Tracing;

internal sealed class TraceInterceptor: Interceptor
{
    private readonly IOrderActivitySource _orderActivitySource;

    public TraceInterceptor(IOrderActivitySource orderActivitySource)
    {
        _orderActivitySource = orderActivitySource;
    }
    
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        using var activity = _orderActivitySource.ActivitySource.StartActivity(
            name: context.Method,
            kind: ActivityKind.Internal,
            tags: new List<KeyValuePair<string, object?>>()
            {
                new ("request", request),
            }
        );

        return base.UnaryServerHandler(request, context, continuation);
    }
}