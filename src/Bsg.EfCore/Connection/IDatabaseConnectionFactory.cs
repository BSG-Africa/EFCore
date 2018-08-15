namespace Bsg.EfCore.Connection
{
    using System.Data.Common;
    using Context;

    public interface IDatabaseConnectionFactory
    {
        DbConnection BuildConnectionForContext<TContext>()
            where TContext : IDbContext;
    }
}