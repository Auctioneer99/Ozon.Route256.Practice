using System.Net;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null && contextFeature.Error != null)
        {
            var errorView = GetErrorView(contextFeature.Error);
            context.Response.StatusCode = (int)errorView.Code;
            context.Response.ContentType = "application/json";
 
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse()
            {
                Error = errorView.Message
            }));
        }
    }
 
    private ErrorView GetErrorView(Exception e)
    {
        switch (e)
        {
            case RpcException rpcException:
                return GetErrorView(rpcException);
            default:
                _logger.LogError(e, "Unhandled exception:" + e.Message);
                return new ErrorView(HttpStatusCode.InternalServerError, e.Message); 
        }
    }

    private ErrorView GetErrorView(RpcException e)
    {
        switch (e.StatusCode)
        {
            case StatusCode.NotFound:
                return new ErrorView(HttpStatusCode.NotFound, e.Status.Detail);
            case StatusCode.InvalidArgument:
                return new ErrorView(HttpStatusCode.BadRequest, e.Status.Detail);
            default:
                _logger.LogError(e, "Unhandled gRPC exception:" + e.Message);
                return new ErrorView(HttpStatusCode.InternalServerError, e.Status.Detail);
        }
    }

    private sealed record ErrorView(HttpStatusCode Code, string Message);
}