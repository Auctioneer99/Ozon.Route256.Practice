﻿using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Services;

namespace Ozon.Route256.Practice.OrdersService.Controllers;

public class OrdersController : OrderService.OrderServiceBase
{
    private readonly RegionService _regionService;

    public OrdersController(RegionService regionService)
    {
        _regionService = regionService;
    }

    public override Task<CancelResponse> CancelOrder(CancelRequest request, ServerCallContext context)
    {
        if (request.Id == 0)
        {
            throw new NotFoundException($"Заказ с Id {request.Id} не найден");
        }

        if (request.Id == 1)
        {
            return Task.FromResult(new CancelResponse
            {
                IsSuccess = false,
                Error = "Заказ на последней стадии оформления"
            });
        }
        
        return Task.FromResult(new CancelResponse
        {
            IsSuccess = true,
            Error = ""
        });
    }

    public override Task<GetStatusByIdResponse> GetStatusById(GetStatusByIdRequest request, ServerCallContext context)
    {
        if (request.Id == 0)
        {
            throw new NotFoundException($"Заказ с Id {request.Id} не найден");
        }

        if (request.Id == 1)
        {
            return Task.FromResult((new GetStatusByIdResponse
            {
                State = Order.Types.OrderState.Delivered
            }));
        }
        
        return Task.FromResult((new GetStatusByIdResponse
        {
            State = Order.Types.OrderState.Created
        }));
    }

    public override Task<GetRegionsResponse> GetRegions(Empty request, ServerCallContext context)
    {
        var response = new GetRegionsResponse();
        response.Regions.AddRange(_regionService.GetRegions());
        
        return Task.FromResult(response);
    }

    public override Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        if (_regionService.HasRegion(request.RegionFilter) == false)
        {
            throw new NotExistsException($"Регион {request.RegionFilter} не существует");
        }
        
        var result = new GetOrdersResponse();

        return Task.FromResult(result);
    }

    public override Task<GetOrdersAggregationResponse> GetOrdersAggregation(GetOrdersAggregationRequest request, ServerCallContext context)
    {
        var notFoundRegions = new HashSet<string>();
        
        foreach (var region in request.Regions)
        {
            if (_regionService.HasRegion(region) == false)
            {
                notFoundRegions.Add(region);
            }
        }

        if (notFoundRegions.Count > 0)
        {
            throw new NotExistsException($"Регионы ({string.Join(", ", notFoundRegions)}) не существуют");
        }

        return Task.FromResult(new GetOrdersAggregationResponse());
    }

    public override Task<GetClientOrdersResponse> GetClientOrders(GetClientOrdersRequest request, ServerCallContext context)
    {
        if (request.ClientId == 0)
        {
            throw new InvalidArgumentException($"Пользователь с ID {request.ClientId} не найден");
        }

        return Task.FromResult(new GetClientOrdersResponse());
    }
}