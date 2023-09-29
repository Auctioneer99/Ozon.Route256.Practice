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
public class OrderController : ControllerBase
{
    private readonly OrdersService.OrderService.OrderServiceClient _orderClient;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrdersService.OrderService.OrderServiceClient orderClient, IMapper mapper, ILogger<OrderController> logger)
    {
        _orderClient = orderClient;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPut("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CancelResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder([FromBody] CancelRequest request)
    {
        try
        {
            var response = await _orderClient.CancelOrderAsync(_mapper.Map<OrdersService.CancelRequest>(request));
            var mappedResponse = _mapper.Map<CancelResponse>(response);

            return mappedResponse.IsSuccess ? Ok() : BadRequest(mappedResponse);
        }
        catch (RpcException e)
        {
            if (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound(new CancelResponse() { IsSuccess = false, Error = e.Status.Detail });
            }
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("status/{orderId}")]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus([FromRoute(Name = "orderId")] long orderId)
    {
        try
        {
            var response = await _orderClient.GetStatusByIdAsync(new OrdersService.GetStatusByIdRequest() { Id = orderId });
            return Ok(_mapper.Map<StatusResponse>(response));
        }
        catch (RpcException e)
        {
            if (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound(new ErrorResponse() { Error = e.Status.Detail} );
            }
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("regions")]
    [ProducesResponseType(typeof(RegionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegions()
    {
        try
        {
            var response = await _orderClient.GetRegionsAsync(new OrdersService.Empty());
            return Ok(_mapper.Map<RegionsResponse>(response));
        }
        catch (RpcException e)
        {
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders([FromQuery] OrdersRequest request)
    {
        try
        {
            var response = await _orderClient.GetOrdersAsync(_mapper.Map<GetOrdersRequest>(request));
            
            return Ok(_mapper.Map<OrdersResponse>(response));
        }
        catch (RpcException e)
        {
            if (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return BadRequest(new ErrorResponse() { Error = e.Status.Detail });
            }
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("aggregation")]
    [ProducesResponseType(typeof(OrdersAggregationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrdersAggregation([FromBody] OrdersAggregationRequest request)
    {
        try
        {
            var response = await _orderClient.GetOrdersAggregationAsync(_mapper.Map<GetOrdersAggregationRequest>(request));
            
            return Ok(_mapper.Map<OrdersAggregationResponse>(response));
        }
        catch (RpcException e)
        {
            if (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return BadRequest(new ErrorResponse() { Error = e.Status.Detail });
            }
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("clientOrders")]
    [ProducesResponseType(typeof(OrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClientOrders([FromQuery] ClientOrdersRequest request)
    {
        try
        {
            var response = await _orderClient.GetClientOrdersAsync(_mapper.Map<GetClientOrdersRequest>(request));
            
            return Ok(_mapper.Map<OrdersResponse>(response));
        }
        catch (RpcException e)
        {
            if (e.StatusCode == Grpc.Core.StatusCode.InvalidArgument)
            {
                return BadRequest(new ErrorResponse() { Error = e.Status.Detail });
            }
            _logger.LogError(e, "gRPC error:" + e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Inner error:" + e.Message);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}