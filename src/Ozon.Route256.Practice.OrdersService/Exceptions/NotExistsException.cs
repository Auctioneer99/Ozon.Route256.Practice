using System.Runtime.Serialization;

namespace Ozon.Route256.Practice.OrdersService.Exceptions;

public sealed class NotExistsException : Exception
{
    public NotExistsException()
    {
    }

    protected NotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NotExistsException(string? message) : base(message)
    {
    }

    public NotExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}