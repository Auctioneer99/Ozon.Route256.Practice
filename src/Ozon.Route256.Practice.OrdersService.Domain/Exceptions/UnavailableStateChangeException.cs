namespace Ozon.Route256.Practice.OrdersService.Exceptions;

public class UnavailableStateChangeException : Exception
{
    public UnavailableStateChangeException(string? message) : base(message)
    {
    }
}