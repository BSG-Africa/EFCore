namespace Bsg.EfCore.Context
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using MoreLinq;
    using SupportType.Dtos;

    /// <summary>
    /// Don't need Strongly Typed DbSets e.g. public DbSet<Type/> DefaultTypes { get; set; }
    /// as the various configurations will setup the model relationships
    /// and the Session and Repository are generic
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public abstract class EfCoreContext : DbContext, IDbContext
    {
        private readonly ContextSupportTypeDto contextSupport;

        protected EfCoreContext(DbContextOptions options, ContextSupportTypeDto contextSupport)
            : base(options)
        {
            this.contextSupport = contextSupport;
        }

        #region Interface Methods

        public bool HasChanges()
        {
            return this.ChangeTracker.HasChanges();
        }

        public DbSet<TEntity> EntitySet<TEntity, TContext>()
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext
        {
            return this.Set<TEntity>();
        }

        #endregion Interface Methods

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            this.AddConfigurations(modelBuilder);

            // these will override instructions in configurations 
            this.AddConventions(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        protected virtual void AddConfigurations(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            foreach (var configType in this.contextSupport.ConfigTypes)
            {
                var entityConfigurationInstance = Activator.CreateInstance(configType.EntityConfigurationType, true);
                configType.GenerifiedModelApplyMethod.Invoke(modelBuilder, new[] { entityConfigurationInstance });
            }
        }

        protected virtual void AddConventions(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            // from https://stackoverflow.com/questions/34768976/specifying-on-delete-no-action-in-entity-framework-7
            // get rid of cascade delete
            modelBuilder
            .Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .ForEach(r =>
            {
                r.DeleteBehavior = DeleteBehavior.Restrict;
            });

            var stringType = typeof(string);

            modelBuilder
                .Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == stringType)
                .ForEach(p =>
                {
                    p.IsUnicode(false);
                });

            // makes all properties (nullables are generic type) required as the default
            modelBuilder
                .Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => !p.ClrType.IsGenericType)
                .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name))
                .ForEach(p =>
                {
                    p.IsRequired();
                });
        }

        #endregion Overrides
    }
}
