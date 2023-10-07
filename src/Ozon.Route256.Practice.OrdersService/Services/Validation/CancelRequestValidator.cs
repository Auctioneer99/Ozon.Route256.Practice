namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class CancelRequestValidator : IValidator<Grpc.Orders.CancelRequest>
{
    public bool Validate(Grpc.Orders.CancelRequest model)
    {
        if (model.Id < 0)
        {
            return false;
        } 
        
        return true;
    }
}