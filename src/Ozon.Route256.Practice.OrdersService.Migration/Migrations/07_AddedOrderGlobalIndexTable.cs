using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Migration.Migrations;

[Migration(7, "AddedOrderGlobalIndexTable")]
public class AddedOrderGlobalIndexTable : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

        create table orders_id_global_index (
            order_id bigint primary key,
            shard int not null
        );

";

    protected override string GetDownSql(IServiceProvider provider) => @"

        drop table orders_id_global_index;

";
}