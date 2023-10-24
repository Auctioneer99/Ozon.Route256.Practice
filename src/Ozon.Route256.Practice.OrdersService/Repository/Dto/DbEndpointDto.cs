namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public class DbEndpointDto
{
    public string HostAndPort { get; init; }
    
    public DbReplicaTypeDto DbReplica { get; init; }
    
    public int Bucket { get; init; }
}