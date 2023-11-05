namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

internal sealed class GetOrdersAggregationRequestValidator : IValidator<Grpc.Orders.GetOrdersAggregationRequest>
{
    public bool Validate(Grpc.Orders.GetOrdersAggregationRequest model)
    {
        if (model.FromDate == null)
        {
            return false;
        }

        if (model.FromDate.ToDateTime() > DateTime.Now)
        {
            return false;
        }

        if (model.FromDate.ToDateTime() < new DateTime(1970, 1, 1))
        {
            return false;
        }

        if (model.Regions == null)
        {
            return false;
        }

        if (model.Regions.Count < 1)
        {
            return false;
        }

        return true;
    }
}