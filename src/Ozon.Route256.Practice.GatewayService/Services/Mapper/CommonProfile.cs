using AutoMapper;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public class CommonProfile : Profile
{
    public CommonProfile()
    {
        CreateMap<PagingRequest, OrdersService.PagingRequest>();

        
        CreateMap<OrdersService.Order.Types.Address, Address>();
        CreateMap<Customers.Address, Address>();
    }
}