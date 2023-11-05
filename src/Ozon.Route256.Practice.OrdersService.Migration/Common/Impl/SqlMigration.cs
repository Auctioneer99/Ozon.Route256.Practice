using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Shard;

namespace Ozon.Route256.Practice.OrdersService.Migration.Common.Impl;

public abstract class SqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        var sqlStatement = GetUpSql(context.ServiceProvider);

        var bucketContext = context.ServiceProvider.GetRequiredService<ShardMigrationContext>();
        var currentSchema = bucketContext.Schema;

        if (context.QuerySchema.SchemaExists(currentSchema) == false)
        {
            context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"create schema {currentSchema};"});
        }
        
        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"set search_path to {currentSchema};"});
        context.Expressions.Add(new ExecuteSqlStatementExpression {SqlStatement = sqlStatement});
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        var sqlStatement = GetDownSql(context.ServiceProvider);
        
        var bucketContext = context.ServiceProvider.GetRequiredService<ShardMigrationContext>();
        var currentSchema = bucketContext.Schema;
        
        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"set search_path to {currentSchema};"});
        context.Expressions.Add(new ExecuteSqlStatementExpression {SqlStatement = sqlStatement});
    }

    public object ApplicationContext { get; }
    public string ConnectionString { get; }

    protected abstract string GetUpSql(IServiceProvider provider);
    protected abstract string GetDownSql(IServiceProvider provider);
}