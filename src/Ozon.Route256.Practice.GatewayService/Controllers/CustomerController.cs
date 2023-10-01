using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CustomerController : ControllerBase
{
    private readonly Customers.Customers.CustomersClient _customersClient;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;

    public CustomerController(Customers.Customers.CustomersClient customersClient, IMapper mapper, ILogger<OrderController> logger)
    {
        _customersClient = customersClient;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var response = await _customersClient.GetCustomersAsync(new Google.Protobuf.WellKnownTypes.Empty());
            
            return Ok(_mapper.Map<CustomersResponse>(response));
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
}