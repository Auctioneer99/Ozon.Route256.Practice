namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public enum OrderState
{
    UndefinedState = 0,
    Created = 1,
    SentToCustomer = 2,
    Delivered = 3,
    Lost = 4,
    Cancelled = 5
}