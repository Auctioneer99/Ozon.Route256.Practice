using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Migration.Migrations;

[Migration(5, "ChangeOrdersType")]
public sealed class ChangeOrdersType  : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

    alter table orders drop column type;
    alter table orders add column type int not null;

    alter table orders drop column state;
    alter table orders add column state int not null;

    drop type ""order"";

    drop type address;

    drop type region;

    drop type order_type;

    drop type order_state;
";

    protected override string GetDownSql(IServiceProvider provider) => @"

    create type order_state as enum ('created', 'sent_to_customer', 'delivered', 'lost', 'cancelled');

    create type order_type as enum ('web', 'mobile', 'api');

    create type region as (
        id bigint,
        name text,
        latitude numeric,
        longitude numeric
    );

    create type address as (
        id bigint, 
        region_id bigint, 
        city text, 
        street text, 
        building text, 
        apartment text, 
        latitude numeric, 
        longitude numeric
    );

    create type ""order"" as (
        id bigint, 
        ""count"" int, 
        total_sum numeric, 
        total_weight numeric, 
        ""type"" order_type, 
        ""state"" order_state, 
        region_from_id bigint, 
        customer_id bigint, 
        address_id bigint, 
        created_at timestamp without time zone
    );

    alter table orders drop column type;
    alter table orders add column type order_type not null;

    alter table orders drop column state;
    alter table orders add column state order_state not null;

";
}