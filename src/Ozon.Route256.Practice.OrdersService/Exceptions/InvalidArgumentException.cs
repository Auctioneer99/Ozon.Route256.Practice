using System.Runtime.Serialization;

namespace Ozon.Route256.Practice.OrdersService.Exceptions;

public class InvalidArgumentException : Exception
{
    public InvalidArgumentException()
    {
    }

    protected InvalidArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidArgumentException(string? message) : base(message)
    {
    }

    public InvalidArgumentException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}