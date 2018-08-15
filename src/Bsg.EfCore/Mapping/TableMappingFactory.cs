namespace Bsg.EfCore.Mapping
{
    using System.Reflection;
    using Context;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Utils;

    public class TableMappingFactory : ITableMappingFactory
    {
        private readonly IGenericReflectorService genericReflectorService;
        private readonly IDbContextFactory contextFactory;

        public TableMappingFactory(
            IGenericReflectorService genericReflectorService,
            IDbContextFactory contextFactory)
        {
            this.genericReflectorService = genericReflectorService;
            this.contextFactory = contextFactory;
        }

        public ContextMapping BuildContextMapping<TContext>()
            where TContext : IDbContext
        {
            var contextTableMappings = new ContextMapping();
            var contextType = typeof(TContext);

            using (var context = this.contextFactory.BuildContext<TContext>())
            {
                foreach (var entityType in context.Model.GetEntityTypes())
                {
                    // can't use lamda to find AddEntityTableMapping because can't specifiy IEntity<IDbContext> because of the new() constraint 
                    var addEntityTableMappingMethod = this.GetType().GetMethod("AddEntityTableMapping", BindingFlags.NonPublic | BindingFlags.Instance); // magic string

                    this.genericReflectorService.InvokeGenericMethodFromMethod(
                        addEntityTableMappingMethod, 
                        new[] { entityType.ClrType, contextType }, 
                        this, 
                        new object[] { contextTableMappings, entityType });
                }
            }

            return contextTableMappings;
        }

        // ReSharper disable once UnusedMember.Local
        private void AddEntityTableMapping<TEntity, TContext>(ContextMapping contextTableMappings, IEntityType entityType)
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext
        {
            var tableMappingForEntity = new TableMapping<TEntity, TContext>();

            foreach (var property in entityType.GetProperties())
            {
                var propertyName = property.PropertyInfo.Name;
                var columnName = property.Relational().ColumnName;
                tableMappingForEntity.ColumnMappings.Add(propertyName, columnName);

                if (property.IsPrimaryKey())
                {
                    tableMappingForEntity.PrimaryKeys.Add(propertyName, columnName);
                }
            }

            tableMappingForEntity.FullyQualifiedTableName = entityType.Relational() != null
                ? $"[{entityType.Relational().Schema}].[{entityType.Relational().TableName}]"
                : $"[dbo].[{entityType.Name}]";

            contextTableMappings.AddMapping(tableMappingForEntity);
        }
    }
}
