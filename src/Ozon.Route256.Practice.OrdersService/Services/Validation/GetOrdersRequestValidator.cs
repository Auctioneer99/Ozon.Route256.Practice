namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class GetOrdersRequestValidator : IValidator<Grpc.Orders.GetOrdersRequest>
{
    public bool Validate(Grpc.Orders.GetOrdersRequest model)
    {
        if (model.Page == null)
        {
            return false;
        }

        if (model.Page.TakeCount < 1)
        {
            return false;
        }

        if (model.Page.SkipCount < 0)
        {
            return false;
        }

        if (model.RegionFilter == null)
        {
            return false;
        }

        if (model.OrderTypeFilter == Grpc.Orders.Order.Types.OrderType.UndefinedType)
        {
            return false;
        }

        return true;
    }
}