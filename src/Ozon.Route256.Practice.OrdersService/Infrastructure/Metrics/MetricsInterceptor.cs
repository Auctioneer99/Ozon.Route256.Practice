using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Metrics;

internal sealed class MetricsInterceptor : Interceptor
{
    private readonly GrpcMetrics _grpcMetrics;

    public MetricsInterceptor(GrpcMetrics grpcMetrics)
    {
        _grpcMetrics = grpcMetrics;
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await base.UnaryServerHandler(request, context, continuation);

            stopwatch.Stop();

            _grpcMetrics.WriteResponseTime(stopwatch.ElapsedMilliseconds, context.Method, isError: false);

            return result;
        }
        catch
        {
            stopwatch.Stop();

            _grpcMetrics.WriteResponseTime(stopwatch.ElapsedMilliseconds, context.Method, isError: true);

            throw;
        }
    }
}