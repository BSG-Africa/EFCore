namespace Bsg.EfCore.Transactions
{
    using System;
    using Context;

    public interface IGenericTransactionService<TContext> 
        where TContext : IDbContext
    {
        void PerformUnitOfWork(Action unitOfWork);

        void PerformUnitOfWork(Action unitOfWork, IDbContextSession<TContext> contextSession);

        TReturn PerformUnitOfWork<TReturn>(Func<TReturn> unitOfWork);

        TReturn PerformUnitOfWork<TReturn>(Func<TReturn> unitOfWork, IDbContextSession<TContext> contextSession);
    }
}