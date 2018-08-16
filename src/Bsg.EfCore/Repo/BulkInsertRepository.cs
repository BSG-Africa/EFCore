namespace Bsg.EfCore.Repo
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Domain;
    using Mapping;
    using Settings;
    using Transactions;
    using Utils;

    public abstract class BulkInsertRepository<TEntity, TContext> : BulkEnabledRepository<TEntity, TContext>, IBulkInsertRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        private readonly IBulkInserterFactory bulkInserterFactory;

        private readonly IDbContextSession<TContext> session;

        private readonly IContextSettingsCacheService contextSettingsCacheService;

        #region Constructor

        protected BulkInsertRepository(
            IDbContextSession<TContext> session,
            IContextSettingsCacheService contextSettingsCacheService,
            IBulkInserterFactory bulkInserterFactory,
            ITableMappingCacheService tableMappingCacheService)
            : base(session, contextSettingsCacheService, tableMappingCacheService)
        {
            this.contextSettingsCacheService = contextSettingsCacheService;
            this.bulkInserterFactory = bulkInserterFactory;
            this.session = session;
        }

        #endregion

        #region Interface Methods

        public int BulkAdd(TEntity item)
        {
            return this.BulkAdd(new[] { item });
        }

        public int BulkAdd(IEnumerable<TEntity> items)
        {
            return this.BulkAdd(items, 5000);
        }

        public int BulkAdd(IEnumerable<TEntity> items, int bufferSize)
        {
            return this.ExecuteAdd(items, bufferSize, null);
        }

        public int BulkAdd(TEntity item, IContextTransaction contextTransaction)
        {
            return this.BulkAdd(new[] { item }, contextTransaction);
        }

        public int BulkAdd(IEnumerable<TEntity> items, IContextTransaction contextTransaction)
        {
            return this.BulkAdd(items, 5000, contextTransaction);
        }

        public int BulkAdd(IEnumerable<TEntity> items, int bufferSize, IContextTransaction contextTransaction)
        {
            return this.ExecuteAdd(items, bufferSize, contextTransaction);
        }

        public IBulkInserter<TEntity> BuildInserter(int bufferSize, IContextTransaction contextTransaction)
        {
            return this.bulkInserterFactory.BuildInserter(
                this.GetMapping(),
                contextTransaction,
                bufferSize,
                this.contextSettingsCacheService.BulkInsertTimeout<TContext>(),
                true);
        }

        private int ExecuteAdd(IEnumerable<TEntity> items, int bufferSize, IContextTransaction contextTransaction)
        {
            if (contextTransaction == null)
            {
                if (this.session.HasCurrentTransaction())
                {
                    return this.ExceuteAddWithExternalTransaction(items, bufferSize, this.session.CurrentTransaction());
                }

                return this.ExecuteAddWithLocalTransaction(items, bufferSize);
            }

            return this.ExceuteAddWithExternalTransaction(items, bufferSize, contextTransaction);
        }

        private int ExecuteAddWithLocalTransaction(IEnumerable<TEntity> items, int bufferSize)
        {
            using (var contextTransaction = this.session.StartNewTransaction())
            {
                try
                {
                    var result = this.ExceuteAddWithExternalTransaction(items, bufferSize, contextTransaction);
                    contextTransaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    contextTransaction?.Rollback();
                    throw;
                }
            }
        }

        private int ExceuteAddWithExternalTransaction(IEnumerable<TEntity> items, int bufferSize, IContextTransaction contextTransaction)
        {
            using (
                var inserter = this.bulkInserterFactory.BuildInserter(
                    this.GetMapping(),
                    contextTransaction,
                    bufferSize,
                    this.contextSettingsCacheService.BulkInsertTimeout<TContext>(),
                    true))
            {
                inserter.Insert(items);
                return inserter.InsertedCount();
            }
        }
        #endregion
    }
}
