namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public class OrderIndexDto
{
    public long OrderId { get; init; }
    
    public int Shard { get; set; }
}