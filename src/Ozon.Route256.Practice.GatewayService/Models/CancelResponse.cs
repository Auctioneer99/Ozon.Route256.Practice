﻿using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class CancelResponse
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; init; }
    
    [JsonProperty("error")]
    public string Error { get; init; }
}