using System.Diagnostics;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Tracing;

internal sealed class OrderActivitySource : IOrderActivitySource
{
    public const string ActivityName = "OrderActivitySource";

    public ActivitySource ActivitySource => new(ActivityName);
}