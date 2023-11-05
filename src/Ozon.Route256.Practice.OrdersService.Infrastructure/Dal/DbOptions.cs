namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;

public sealed class DbOptions
{
    public string ClusterName { get; init; }
    
    public string DatabaseName { get; init; }
    
    public string User { get; init; }
    
    public string Password { get; init; }
}