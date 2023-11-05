namespace Ozon.Route256.Practice.OrdersService.Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string? message) : base(message)
    {
    }
}