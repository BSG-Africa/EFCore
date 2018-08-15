namespace Bsg.EfCore.Context
{
    public interface IDbContextFactory
    {
        IDbContext BuildContext<TContext>()
            where TContext : IDbContext;
    }
}