namespace Bsg.EfCore.Transactions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Context;

    public class GenericTransactionService<TContext> : IGenericTransactionService<TContext>
        where TContext : IDbContext
    {
        private readonly IDbContextSession<TContext> session;

        public GenericTransactionService(IDbContextSession<TContext> session)
        {
            this.session = session;
        }

        public void PerformUnitOfWork(Action unitOfWork)
        {
            this.PerformWork(unitOfWork, this.session);
        }

        public void PerformUnitOfWork(Action unitOfWork, IDbContextSession<TContext> contextSession)
        {
            this.PerformWork(unitOfWork, contextSession);
        }

        public TReturn PerformUnitOfWork<TReturn>(Func<TReturn> unitOfWork)
        {
            return this.PerformWork(unitOfWork, this.session);
        }

        public TReturn PerformUnitOfWork<TReturn>(Func<TReturn> unitOfWork, IDbContextSession<TContext> contextSession)
        {
            return this.PerformWork(unitOfWork, contextSession);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void PerformWork(Action unitOfWork, IDbContextSession<TContext> contextSession)
        {
            if (contextSession.HasCurrentTransaction())
            {
                unitOfWork();
                contextSession.CommitChanges();
            }
            else
            {
                using (var transaction = contextSession.StartNewTransaction())
                {
                    try
                    {
                        unitOfWork();
                        contextSession.CommitChanges();
                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            //swallow
                        }

                        try
                        {
                            contextSession.RevertChanges();
                        }
                        catch
                        {
                            // swallow;
                        }

                        // throw original
                        throw;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private TReturn PerformWork<TReturn>(Func<TReturn> unitOfWork, IDbContextSession<TContext> contextSession)
        {
            TReturn returnValue;

            if (contextSession.HasCurrentTransaction())
            {
                returnValue = unitOfWork();
                contextSession.CommitChanges();
            }
            else
            {
                using (var transaction = contextSession.StartNewTransaction())
                {
                    try
                    {
                        returnValue = unitOfWork();
                        contextSession.CommitChanges();
                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            //swallow
                        }

                        try
                        {
                            contextSession.RevertChanges();
                        }
                        catch
                        {
                            // swallow;
                        }

                        // throw original
                        throw;
                    }
                }
            }

            return returnValue;
        }
    }
}