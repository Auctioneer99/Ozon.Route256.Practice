using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

/*
[Migration(4, "AddedGlobalIndexTable")]
public class AddedGlobalIndexTable : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

        create table orders_id_global_index (
            order_id bigint primary key,
            cluster_index int not null;
        );

";

    protected override string GetDownSql(IServiceProvider provider) => @"

        drop table orders_id_global_index;

";
}*/