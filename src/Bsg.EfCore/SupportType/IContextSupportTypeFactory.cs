namespace Bsg.EfCore.SupportType
{
    using Context;
    using SupportType.Dtos;

    public interface IContextSupportTypeFactory
    {
        ContextSupportTypeDto BuildContextTypes<TContext>() 
            where TContext : IDbContext;
    }
}