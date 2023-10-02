namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class GetClientOrdersRequestValidator : IValidator<GetClientOrdersRequest>
{
    public bool Validate(GetClientOrdersRequest model)
    {
        if (model.ClientId < 0)
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

        if (model.From.ToDateTime() < new DateTime(2010, 1, 1))
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