namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class GetOrdersAggregationRequestValidator : IValidator<GetOrdersAggregationRequest>
{
    public bool Validate(GetOrdersAggregationRequest model)
    {
        if (model.FromDate == null)
        {
            return false;
        }

        if (model.FromDate.ToDateTime() > DateTime.Now)
        {
            return false;
        }

        if (model.FromDate.ToDateTime() < new DateTime(2010, 1, 1))
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