namespace Ozon.Route256.Practice.OrdersService.Services.Validation;

public interface IValidator<TRequest>
{
    public bool Validate(TRequest model);
}