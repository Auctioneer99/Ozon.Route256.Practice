using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Migration.Migrations;

[Migration(3, "InvertingAddressesRelationship")]
public sealed class InvertingAddressesRelationship : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

    alter table addresses add column order_id bigint not null;

    alter table orders drop column address_id;

";

    protected override string GetDownSql(IServiceProvider provider) => @"

    alter table orders add column address_id bigint not null;

    alter table addresses drop column order_id;

";
}