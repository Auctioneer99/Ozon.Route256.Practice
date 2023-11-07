using Grpc.Core;
using Grpc.Core.Interceptors;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Services.Validation;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure;

internal sealed class ValidatorInterceptor: Interceptor
{
    private readonly ILogger<ValidatorInterceptor> _logger;

    public ValidatorInterceptor(ILogger<ValidatorInterceptor> logger)
    {
        _logger = logger;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validator = context.GetHttpContext().RequestServices.GetService<IValidator<TRequest>>();

        if (validator == null)
        {
            _logger.LogWarning($"Возможно не зарегистрирован валидатор для сущности {typeof(TRequest).Name}");
        }
        
        if (validator != null && validator.Validate(request) == false)
        {
            throw new InvalidArgumentException("При проверке полей, возникли ошибки");
        }
        
        return base.UnaryServerHandler(request, context, continuation);
    }
}