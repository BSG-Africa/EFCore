namespace Bsg.EfCore.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;
    using Context;
    using Domain;

    public class TableMapping<TEntity, TContext> 
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        private readonly IDictionary<string, Func<TEntity, object>> propertyCache;

        public TableMapping()
        {
            this.ColumnMappings = new Dictionary<string, string>();
            this.propertyCache = new Dictionary<string, Func<TEntity, object>>();
            this.CachePropertyGetDelegates();
            this.PrimaryKeys = new Dictionary<string, string>();
        }

        public string FullyQualifiedTableName { get; set; }

        public IDictionary<string, string> ColumnMappings { get; }

        public IDictionary<string, string> PrimaryKeys { get; }

        public bool HasPrimaryKeys => this.PrimaryKeys.Count > 0;

        public void MapRow(DataRow row, TEntity instance)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            foreach (var propertyName in this.propertyCache.Keys)
            {
                var columnName = propertyName;

                if (this.ColumnMappings.ContainsKey(propertyName))
                {
                    columnName = this.ColumnMappings[propertyName];
                }

                if (row.Table.Columns.Contains(columnName))
                {
                    row[columnName] = this.propertyCache[propertyName](instance) ?? DBNull.Value;
                }
            }

            if (row.Table.Columns.Contains("Discriminator"))
            {
                row["Discriminator"] = typeof(TEntity).Name;
            }
        }

        private void CachePropertyGetDelegates()
        {
            var properties =
                typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (propertyType.IsValueType || propertyType == typeof(string) || propertyType == typeof(byte[]))
                {
                    var preCompiledFunction = this.BuildPropertyGetDelegate(property);
                    this.propertyCache.Add(property.Name, preCompiledFunction);
                }
            }
        }

        private Func<TEntity, object> BuildPropertyGetDelegate(PropertyInfo property)
        {
            var entityType = typeof(TEntity);
            var parameterExpression = Expression.Parameter(entityType, "x");
            var memberExpression = Expression.Property(parameterExpression, property);
            var convertedUnaryExpression = Expression.Convert(memberExpression, typeof(object));
            var lamdaExpression = Expression.Lambda(convertedUnaryExpression, parameterExpression);
            return ((Expression<Func<TEntity, object>>)lamdaExpression).Compile();
        }
    }
}
