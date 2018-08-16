namespace Bsg.EfCore.Repo
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using Context;
    using Domain;
    using Mapping;
    using Settings;
    using Transactions;

    public abstract class BulkEnabledRepository<TEntity, TContext> : Repository<TEntity, TContext>, IBulkEnabledRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        private readonly IDbContextSession<TContext> session;

        private readonly IContextSettingsCacheService contextSettingsCacheService;

        private readonly ITableMappingCacheService tableMappingCacheService;

        protected BulkEnabledRepository(
            IDbContextSession<TContext> session,
            IContextSettingsCacheService contextSettingsCacheService,
            ITableMappingCacheService tableMappingCacheService)
            : base(session)
        {
            this.tableMappingCacheService = tableMappingCacheService;
            this.contextSettingsCacheService = contextSettingsCacheService;
            this.session = session;
        }

        #region Interface Methods

        public int Truncate()
        {
            return this.Truncate(null);
        }

        public int Truncate(IContextTransaction contextTransaction)
        {
            var tableName = this.GetMapping().FullyQualifiedTableName;

            var truncateQuery = $"TRUNCATE TABLE {tableName}";

            return this.Execute(truncateQuery, null, contextTransaction);
        }

        public int TruncateWithForeignKeys()
        {
            return this.TruncateWithForeignKeys(null);
        }

        public int TruncateWithForeignKeys(IContextTransaction contextTransaction)
        {
            var tableName = this.GetMapping().FullyQualifiedTableName;

            // Adapted from http://stackoverflow.com/questions/472578/dbcc-checkident-sets-identity-to-0 
            // Issue when previously no records and reseed to 0, will start ids at 0 instead of 1
            // Changing RESEED to 1, could potentially start ids at 2
            // Logic below checks if the Id column has already been seeded and if so then reseeds to 0, which will result in the next Id of 1
            // if Id column has not been used then the RESEED is not required, as the next Id will be 1 anyway
            var truncateQuery = $"DELETE FROM {tableName}; IF EXISTS (SELECT * FROM sys.identity_columns WHERE QUOTENAME(OBJECT_SCHEMA_NAME(object_id)) + '.' + QUOTENAME(OBJECT_NAME(object_id)) = '{tableName}' AND last_value IS NOT NULL) DBCC CHECKIDENT ('{tableName}',RESEED, 0);";

            return this.Execute(truncateQuery, null, contextTransaction);
        }
        #endregion

        #region Protected Methods

        protected TableMapping<TEntity, TContext> GetMapping()
        {
            return this.GetMapping<TEntity>();
        } 

        protected int Execute(
           string sql,
           IEnumerable<PlaceHolderObjectParameterDto> parameters,
           IContextTransaction contextTransaction)
        {
            if (contextTransaction == null)
            {
                if (this.session.HasCurrentTransaction())
                {
                    return this.ExecuteWithExternalTransaction(sql, parameters, this.session.CurrentTransaction());
                }

                return this.ExecuteWithLocalTransaction(sql, parameters);
            }

            return this.ExecuteWithExternalTransaction(sql, parameters, contextTransaction);
        }

        protected int ExecuteWithLocalTransaction(
            string sql,
            IEnumerable<PlaceHolderObjectParameterDto> parameters)
        {
            using (var contextTransaction = this.session.StartNewTransaction())
            {
                try
                {
                    var result = this.ExecuteWithExternalTransaction(sql, parameters, contextTransaction);
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

        protected int ExecuteWithExternalTransaction(
            string sql,
            IEnumerable<PlaceHolderObjectParameterDto> parameters,
            IContextTransaction contextTransaction)
        {
            if (contextTransaction == null)
            {
                throw new ArgumentNullException(nameof(contextTransaction));
            }

            var sqlTransaction = contextTransaction.UnderlyingTransaction<SqlTransaction>();

            if (sqlTransaction == null)
            {
                throw new InvalidOperationException("Bulk Inserter requires a SQL Transaction");
            }

            var sqlConnection = sqlTransaction.Connection;

            return this.ExecuteBulkNonQuery(sql, parameters, sqlTransaction, sqlConnection);
        }

        #endregion

        #region Private Methods

        private TableMapping<TClass, TContext> GetMapping<TClass>() 
            where TClass : class, IEntity<TContext>, new()
        {
            var mappings = this.tableMappingCacheService.RetrieveContextMapping<TContext>();
            return mappings.GetMapping<TClass, TContext>();
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int ExecuteBulkNonQuery(
            string sql,
            IEnumerable<PlaceHolderObjectParameterDto> parameters,
            SqlTransaction sqlTransaction,
            SqlConnection sqlConnection)
        {
            var command = sqlConnection.CreateCommand();
            command.Transaction = sqlTransaction;
            command.CommandText = sql;
            command.CommandTimeout = this.contextSettingsCacheService.BulkUpdateTimeout<TContext>();

            if (parameters != null)
            {
                // parameters are used when strings are used in the source IQueryable
                foreach (var objectParameter in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = objectParameter.Name;
                    parameter.Value = objectParameter.Value;
                    command.Parameters.Add(parameter);
                }
            }
          
            return command.ExecuteNonQuery();
        }
    }
        #endregion
}