namespace Bsg.EfCore.Domain
{
    using Context;

    public interface IEntity<TContext>
        where TContext : IDbContext
    {
    }
}