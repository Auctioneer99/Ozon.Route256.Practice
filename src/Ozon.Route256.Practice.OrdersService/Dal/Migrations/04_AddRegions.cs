using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Impl;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

[Migration(4, "AddRegions")]
public sealed class AddRegions : SqlMigration 
{
    protected override string GetUpSql(IServiceProvider provider) => @"

    insert into regions values 
        (1, 'Moscow', 55.7522, 37.6156),
        (2, 'StPetersburg', 55.01, 82.55),
        (3, 'Novosibirsk', 45.32, 68.23);

";

    protected override string GetDownSql(IServiceProvider provider) => @"

    delete from regions where id in (1, 2, 3);

";
}