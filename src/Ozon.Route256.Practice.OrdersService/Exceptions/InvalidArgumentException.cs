namespace Ozon.Route256.Practice.OrdersService.Exceptions;

internal sealed class InvalidArgumentException : Exception
{
    public InvalidArgumentException(string? message) : base(message)
    {
    }
}