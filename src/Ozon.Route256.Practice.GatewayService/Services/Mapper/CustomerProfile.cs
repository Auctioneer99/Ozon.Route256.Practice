using AutoMapper;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customers.GetCustomersResponse, CustomersResponse>()
            .ForMember(d => d.Customers, opt => opt.MapFrom(o => o.Customers.Map()));
    }
}