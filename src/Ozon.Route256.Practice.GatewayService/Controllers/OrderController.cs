using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Ozon.Route256.Practice.OrdersService;
using CancelRequest = Ozon.Route256.Practice.GatewayService.Models.CancelRequest;
using CancelResponse = Ozon.Route256.Practice.GatewayService.Models.CancelResponse;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class OrderController : ControllerBase
{
    private readonly Orders.OrdersClient _orderClient;
    private readonly IMapper _mapper;

    public OrderController(Orders.OrdersClient orderClient, IMapper mapper)
    {
        _orderClient = orderClient;
        _mapper = mapper;
    }

    [HttpPut("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status404NotFound)]
    public async Task CancelOrder([FromBody] CancelRequest request)
    {
        await _orderClient.CancelOrderAsync(_mapper.Map<OrdersService.CancelRequest>(request));
    }

    [HttpGet("status/{orderId}")]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<StatusResponse> GetStatus([FromRoute(Name = "orderId")] long orderId)
    {
        var response =  await _orderClient.GetStatusByIdAsync(new OrdersService.GetStatusByIdRequest() { Id = orderId });
        return _mapper.Map<StatusResponse>(response);
    }

    [HttpGet("regions")]
    [ProducesResponseType(typeof(RegionsResponse), StatusCodes.Status200OK)]
    public async Task<RegionsResponse> GetRegions()
    {
        var response = await _orderClient.GetRegionsAsync(new OrdersService.Empty());
        return _mapper.Map<RegionsResponse>(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<OrdersResponse> GetOrders([FromQuery] OrdersRequest request)
    {
        var response = await _orderClient.GetOrdersAsync(_mapper.Map<GetOrdersRequest>(request));
        return _mapper.Map<OrdersResponse>(response);
    }

    [HttpPost("aggregation")]
    [ProducesResponseType(typeof(OrdersAggregationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<OrdersAggregationResponse> GetOrdersAggregation([FromBody] OrdersAggregationRequest request)
    {
        var response = await _orderClient.GetOrdersAggregationAsync(_mapper.Map<GetOrdersAggregationRequest>(request));
        return _mapper.Map<OrdersAggregationResponse>(response);
    }

    [HttpGet("clientOrders")]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<OrdersResponse> GetClientOrders([FromQuery] ClientOrdersRequest request)
    {
        var response = await _orderClient.GetClientOrdersAsync(_mapper.Map<GetClientOrdersRequest>(request));
        return _mapper.Map<OrdersResponse>(response);
    }
}