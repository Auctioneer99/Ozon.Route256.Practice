namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

internal sealed class GetCustomerOrdersRequestValidator : IValidator<Grpc.Orders.GetCustomerOrdersRequest>
{
    public bool Validate(Grpc.Orders.GetCustomerOrdersRequest model)
    {
        if (model.CustomerId < 0)
        {
            return false;
        }

        if (model.From == null)
        {
            return false;
        }

        if (model.From.ToDateTime() > DateTime.Now)
        {
            return false;
        }

        if (model.From.ToDateTime() < new DateTime(1970, 1, 1))
        {
            return false;
        }

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
        
        return true;
    }
}