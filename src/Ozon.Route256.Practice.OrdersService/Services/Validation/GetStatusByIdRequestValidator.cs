namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

internal sealed class GetStatusByIdRequestValidator : IValidator<Grpc.Orders.GetStatusByIdRequest>
{
    public bool Validate(Grpc.Orders.GetStatusByIdRequest model)
    {
        if (model.Id < 0)
        {
            return false;
        } 

        return true;
    }
}