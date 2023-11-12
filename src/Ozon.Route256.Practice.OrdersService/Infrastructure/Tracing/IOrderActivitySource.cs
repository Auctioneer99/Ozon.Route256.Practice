using System.Diagnostics;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Tracing;

public interface IOrderActivitySource
{
    ActivitySource ActivitySource { get; }
}