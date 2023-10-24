using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

[Migration(2, "InitIndexes")]
public sealed class InitIndexes : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

    create unique index regions_name_unique_index on regions (name);

    create index orders_customer_id_index on orders (customer_id);

    create index orders_type_region_from_id_index on orders (type, region_from_id);

    create index addresses_latitude_longitude_index on addresses (latitude, longitude);

";

    protected override string GetDownSql(IServiceProvider provider) => @"

    drop index addresses_latitude_longitude_index;

    drop index orders_type_region_from_id_index;

    drop index orders_customer_id_index;

    drop index regions_name_unique_index;

";
}