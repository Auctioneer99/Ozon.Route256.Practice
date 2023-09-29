using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.GatewayService.Models;
using CancelRequest = Ozon.Route256.Practice.GatewayService.Models.CancelRequest;
using CancelResponse = Ozon.Route256.Practice.GatewayService.Models.CancelResponse;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrdersService.Order, Order>();
        
        CreateMap<CancelRequest, OrdersService.CancelRequest>();
        CreateMap<OrdersService.CancelResponse, CancelResponse>();

        CreateMap<OrdersService.GetStatusByIdResponse, StatusResponse>()
            .ForMember(d => d.Status, opt => opt.MapFrom(o => o.State.ToString()));

        CreateMap<OrdersService.GetRegionsResponse, RegionsResponse>()
            .ForMember(d => d.Regions, opt => opt.MapFrom(o => o.Regions.Map()));

        CreateMap<OrdersRequest, OrdersService.GetOrdersRequest>();
        CreateMap<OrdersService.GetOrdersResponse, OrdersResponse>()
            .ForMember(d => d.Orders, opt => opt.MapFrom(o => o.Orders.Map()));
        
        CreateMap<OrdersAggregationRequest, OrdersService.GetOrdersAggregationRequest>()
            .ForMember(d => d.FromDate, opt => opt.MapFrom(o => o.FromDate.ToUniversalTime().ToTimestamp()))
            .ForMember(d => d.Regions, opt => opt.MapFrom(o => o.Regions.ToRepeated()));
        CreateMap<OrdersService.GetOrdersAggregationResponse, OrdersAggregationResponse>()
            .ForMember(d => d.Aggregations, opt => opt.MapFrom(o => o.Aggregations.Map()));
        CreateMap<OrdersService.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry,
            OrdersAggregationResponse.OrdersAggregationResponseEntry>();

        CreateMap<ClientOrdersRequest, OrdersService.GetClientOrdersRequest>()
            .ForMember(d => d.From, opt => opt.MapFrom(o => o.From.ToUniversalTime().ToTimestamp()));
        CreateMap<OrdersService.GetClientOrdersResponse, OrdersResponse>()
            .ForMember(d => d.Orders, opt => opt.MapFrom(o => o.Orders.Map()));
    }
}