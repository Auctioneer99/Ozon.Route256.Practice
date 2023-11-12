namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;

public sealed class DbOptions
{
    public string ClusterName { get; init; } = null!;

    public string DatabaseName { get; init; } = null!;

    public string User { get; init; } = null!;

    public string Password { get; init; } = null!;
}