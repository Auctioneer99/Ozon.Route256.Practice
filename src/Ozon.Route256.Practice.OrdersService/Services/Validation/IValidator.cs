namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

internal interface IValidator<TRequest>
{
    public bool Validate(TRequest model);
}