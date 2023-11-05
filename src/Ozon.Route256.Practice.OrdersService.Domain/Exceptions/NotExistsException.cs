namespace Ozon.Route256.Practice.OrdersService.Domain.Exceptions;

public sealed class NotExistsException : Exception
{
    public NotExistsException(string? message) : base(message)
    {
    }
}