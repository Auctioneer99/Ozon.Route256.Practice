using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Ozon.Route256.Practice.GatewayService.Services.Mapper;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class OrderController : ControllerBase
{
    private readonly Grpc.Orders.Orders.OrdersClient _orderClient;

    public OrderController(Grpc.Orders.Orders.OrdersClient orderClient)
    {
        _orderClient = orderClient;
    }

    [HttpPut("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status404NotFound)]
    public async Task CancelOrder([FromBody] CancelRequest request)
    {
        await _orderClient.CancelOrderAsync(request.FromDto());
    }

    [HttpGet("status/{orderId}")]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<StatusResponse> GetStatus([FromRoute(Name = "orderId")] long orderId)
    {
        var response = await _orderClient.GetStatusByIdAsync(new Grpc.Orders.GetStatusByIdRequest() { Id = orderId });
        return response.ToDto();
    }

    [HttpGet("regions")]
    [ProducesResponseType(typeof(RegionsResponse), StatusCodes.Status200OK)]
    public async Task<RegionsResponse> GetRegions()
    {
        var response = await _orderClient.GetRegionsAsync(new Grpc.Orders.Empty());
        return response.ToDto();
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<OrdersResponse> GetOrders([FromQuery] OrdersRequest request)
    {
        var response = await _orderClient.GetOrdersAsync(request.FromDto());
        return response.ToDto();
    }

    [HttpPost("aggregation")]
    [ProducesResponseType(typeof(OrdersAggregationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<OrdersAggregationResponse> GetOrdersAggregation([FromBody] OrdersAggregationRequest request)
    {
        var response = await _orderClient.GetOrdersAggregationAsync(request.FromDto());
        return response.ToDto();
    }

    [HttpGet("clientOrders")]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<OrdersResponse> GetCustomerOrders([FromQuery] CustomerOrdersRequest request)
    {
        var response = await _orderClient.GetCustomerOrdersAsync(request.FromDto());
        return response.ToDto();
    }
}