using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Migration.Migrations;

[Migration(6, "AddCustomerColumnToAddress")]
public class AddCustomerColumnToAddress : SqlMigration
{
    protected override string GetUpSql(IServiceProvider provider) => @"

    alter table addresses add column customer_id bigint not null;

";

    protected override string GetDownSql(IServiceProvider provider) => @"

    alter table addresses drop column customer_id;

";
}