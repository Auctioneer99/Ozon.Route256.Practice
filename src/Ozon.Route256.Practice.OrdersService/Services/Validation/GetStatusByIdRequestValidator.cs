namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public sealed class GetStatusByIdRequestValidator : IValidator<GetStatusByIdRequest>
{
    public bool Validate(GetStatusByIdRequest model)
    {
        if (model.Id < 0)
        {
            return false;
        } 

        return true;
    }
}