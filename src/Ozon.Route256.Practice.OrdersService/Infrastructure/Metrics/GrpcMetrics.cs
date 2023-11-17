using Prometheus;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Metrics;

internal sealed class GrpcMetrics
{
    private readonly Histogram _histogram = Prometheus.Metrics.CreateHistogram(
        name: "orderservice_grpc_response_time",
        help: "Время ответа сервиса",
        labelNames: new[] { "methodName", "isError" });

    public void WriteResponseTime(long elapsedMilliseconds, string methodName, bool isError) =>
        _histogram.WithLabels(methodName, isError ? "1" : "0").Observe((double)elapsedMilliseconds / 1000);
}