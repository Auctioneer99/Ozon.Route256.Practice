using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Ozon.Route256.Practice.GatewayService.Services.Mapper;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CustomerController : ControllerBase
{
    private readonly Customers.Customers.CustomersClient _customersClient;

    public CustomerController(Customers.Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomersResponse), StatusCodes.Status200OK)]
    public async Task<CustomersResponse> GetCustomers()
    {
        var response = await _customersClient.GetCustomersAsync(new Google.Protobuf.WellKnownTypes.Empty());
        return response.ToDto();
    }
}