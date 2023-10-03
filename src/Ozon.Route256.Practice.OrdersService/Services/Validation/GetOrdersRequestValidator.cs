namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class GetOrdersRequestValidator : IValidator<GetOrdersRequest>
{
    public bool Validate(GetOrdersRequest model)
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

        if (model.OrderTypeFilter == Order.Types.OrderType.UndefinedType)
        {
            return false;
        }

        return true;
    }
}