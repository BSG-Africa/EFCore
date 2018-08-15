/*
* Heavily modified from https://github.com/ronnieoverby/RonnieOverbyGrabBag
*/

namespace Bsg.EfCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Context;
    using Domain;
    using Mapping;
    using Transactions;

    public class BulkInserter<TEntity, TContext> : IBulkInserter<TEntity>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        #region Private Members

        private readonly object loclObj;
        
        private readonly int bufferSize;

        private readonly int timeout;

        private readonly IList<TEntity> queue;

        private readonly IContextTransaction contextTransaction;

        private readonly TableMapping<TEntity, TContext> tableMapping;

        private bool isInitilaised;
 
        private int noOfInsertedItems;

        private DataTable dataTable;

        #endregion

        #region Constructors

        public BulkInserter(TableMapping<TEntity, TContext> tableMapping, IContextTransaction contextTransaction, int bufferSize, int timeout, bool preInitialise)
        {
            this.noOfInsertedItems = 0;
            this.bufferSize = bufferSize;
            this.timeout = timeout;
            this.loclObj = new object();
            this.queue = new List<TEntity>();
            this.contextTransaction = contextTransaction;
            this.tableMapping = tableMapping;

            if (preInitialise)
            {
                this.EnsureInitialised();
            }
        }

        #endregion Constructors

        public int InsertedCount()
        {
            return this.noOfInsertedItems;
        }

        public void ResetInsertedCount()
        {
            this.noOfInsertedItems = 0;
        }

        public void Insert(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.EnsureInitialised();

            var connectionDetails = this.GetConnectionDetails();
            var connection = connectionDetails.Item1;
            var transaction = connectionDetails.Item2;
            this.Insert(items, connection, transaction);
        }

        public void Insert(TEntity item)
        {
            this.Insert(new[] { item });
        }

        public void Queue(TEntity item, bool autoFlush)
        {
            this.queue.Add(item);

            if (autoFlush && this.queue.Count == this.bufferSize)
            {
                this.Flush();
            }
        }

        public void Flush()
        {
            this.Insert(this.queue);
            this.queue.Clear();
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
                if (this.isInitilaised)
                {
                    this.dataTable?.Dispose();
                }
            }
        }

        #region Private Methods

        private IEnumerable<IList<TEntity>> BufferGroups(IEnumerable<TEntity> items)
        {
            var enumerator = items.GetEnumerator();
            var buffer = this.bufferSize;
            var hasMoreItems = true;

            while (hasMoreItems)
            {
                var bufferGroup = new List<TEntity>(buffer);

                while (buffer > bufferGroup.Count && (hasMoreItems = enumerator.MoveNext()))
                {
                    bufferGroup.Add(enumerator.Current);
                }

                if (bufferGroup.Count > 0)
                {
                    yield return bufferGroup;
                }
            }
        }

        private void EnsureInitialised()
        {
            if (!this.isInitilaised)
            {
                lock (this.loclObj)
                {
                    if (!this.isInitilaised)
                    {
                        var connectionDetails = this.GetConnectionDetails();
                        var connection = connectionDetails.Item1;
                        var transaction = connectionDetails.Item2;

                        // Gets table structure
                        this.dataTable = this.GetTableStructure(connection, transaction);

                        // sets up table
                        this.RemoveReadOnlyNonAutoIncrementedColumns();

                        this.isInitilaised = true;
                    }
                }
            }
        }

        private void Insert(IEnumerable<TEntity> items, SqlConnection openConnection, SqlTransaction sqlTransaction)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(openConnection, SqlBulkCopyOptions.CheckConstraints, sqlTransaction))
            {
                sqlBulkCopy.DestinationTableName = this.tableMapping.FullyQualifiedTableName;
                sqlBulkCopy.BulkCopyTimeout = this.timeout;
                sqlBulkCopy.BatchSize = this.bufferSize;

                foreach (var bufferGroup in this.BufferGroups(items))
                {
                    foreach (var item in bufferGroup)
                    {
                        var row = this.dataTable.NewRow();
                        this.tableMapping.MapRow(row, item);
                        this.dataTable.Rows.Add(row);
                    }

                    sqlBulkCopy.WriteToServer(this.dataTable);
                    this.noOfInsertedItems += bufferGroup.Count;
                    this.dataTable.Clear();
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")] 
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private DataTable GetTableStructure(SqlConnection openConnection, SqlTransaction sqlTransaction)
        {
            var table = new DataTable { Locale = CultureInfo.InvariantCulture };

            using (var command = openConnection.CreateCommand())
            {
                command.Transaction = sqlTransaction;
                command.CommandText = $"select top 0 * from {this.tableMapping.FullyQualifiedTableName}";

                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        private void RemoveReadOnlyNonAutoIncrementedColumns()
        {
            for (var columnIdx = this.dataTable.Columns.Count - 1; columnIdx >= 0; columnIdx--)
            {
                var column = this.dataTable.Columns[columnIdx];

                if (column.ReadOnly && !column.AutoIncrement)
                {
                    // Remove contraints on the column
                    for (var constraintIdx = this.dataTable.Constraints.Count - 1; constraintIdx >= 0; constraintIdx--)
                    {
                        // TODO other potential column types
                        var uniqueContraint = this.dataTable.Constraints[constraintIdx] as UniqueConstraint;

                        if (uniqueContraint != null && uniqueContraint.Columns.ToList().Contains(column))
                        {
                            this.dataTable.Constraints.RemoveAt(constraintIdx);
                            break;
                        }
                    }

                    this.dataTable.Columns.Remove(column);
                }
            }
        }

        private Tuple<SqlConnection, SqlTransaction> GetConnectionDetails()
        {
            var sqlTransaction = this.contextTransaction.UnderlyingTransaction<SqlTransaction>();

            if (sqlTransaction == null)
            {
                throw new InvalidOperationException("Bulk Inserter requires a SQL Transaction");
            }

            var sqlConnection = sqlTransaction.Connection;

            return new Tuple<SqlConnection, SqlTransaction>(sqlConnection, sqlTransaction);
        }

        #endregion
    }
}