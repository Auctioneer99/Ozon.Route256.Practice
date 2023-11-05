namespace Ozon.Route256.Practice.OrdersService.Dal.Common;

public sealed class DbOptions
{
    public string ClusterName { get; init; }
    
    public string DatabaseName { get; init; }
    
    public string User { get; init; }
    
    public string Password { get; init; }
}