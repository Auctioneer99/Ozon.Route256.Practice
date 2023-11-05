using Grpc.Core;
using Grpc.Core.Interceptors;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure;

internal sealed class LoggerInterceptor: Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger;

    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Запрос {request}", request);
        try
        {
            var response = await base.UnaryServerHandler(request, context, continuation);
            _logger.LogInformation("Ответ {response}", response);
            return response;
        }
        catch (NotFoundException e)
        {
            _logger.LogError(e, "Не найден");
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (NotExistsException e)
        {
            _logger.LogError(e, "Не существует");
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (InvalidArgumentException e)
        {
            _logger.LogError(e, "Неверный аргумент");
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Возникла необработанная ошибка");
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
    }
}
