namespace Bsg.EfCore.Transactions
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore.Storage;

    public class ContextTransaction : IContextTransaction
    {
        private readonly IDbContextTransaction dbContextTransaction;

        public ContextTransaction(IDbContextTransaction dbContextTransaction)
        {
            this.dbContextTransaction = dbContextTransaction;
        }

        public TTransactionType UnderlyingTransaction<TTransactionType>()
            where TTransactionType : DbTransaction
        {
            return this.dbContextTransaction.GetDbTransaction() as TTransactionType;
        }

        public void Rollback()
        {
            this.dbContextTransaction.Rollback();
        }

        public void Commit()
        {
            this.dbContextTransaction.Commit();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dbContextTransaction.Dispose();
            }
        }
    }
}
