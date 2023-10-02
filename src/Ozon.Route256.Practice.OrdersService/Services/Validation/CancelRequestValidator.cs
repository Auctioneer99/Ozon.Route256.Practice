namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class CancelRequestValidator : IValidator<CancelRequest>
{
    public bool Validate(CancelRequest model)
    {
        if (model.Id < 0)
        {
            return false;
        } 
        
        return true;
    }
}