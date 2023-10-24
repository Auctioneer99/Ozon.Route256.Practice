﻿namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;

public static class Shards
{
    public const string BucketPlaceholder = "__bucket__";
    
    public static string GetSchemaName(int bucketId) => $"bucket_{bucketId}";
}