using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

[Migration(1, "Init")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

        create table regions (
            id bigserial primary key,
            name text not null,
            latitude numeric not null,
            longitude numeric not null
        );

        create table addresses (
            id bigserial primary key, 
            region_id bigint not null, 
            city text not null, 
            street text not null, 
            building text not null, 
            apartment text not null, 
            latitude numeric not null, 
            longitude numeric not null
        );

        create type order_state as enum ('created', 'sent_to_customer', 'delivered', 'lost', 'cancelled');

        create type order_type as enum ('web', 'mobile', 'api');

        create table orders (
            id bigserial primary key, 
            ""count"" int not null, 
            total_sum numeric not null, 
            total_weight numeric not null, 
            ""type"" order_type not null, 
            ""state"" order_state not null, 
            region_from_id bigint not null, 
            customer_id bigint not null, 
            address_id bigint not null, 
            created_at timestamp without time zone not null
        );

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

        ";

    protected override string GetDownSql(IServiceProvider provider) => @"

        drop type ""order"";

        drop type address;

        drop type region;

        drop table orders;

        drop type order_type;

        drop type order_state;

        drop table addresses;

        drop table regions;

        ";
}