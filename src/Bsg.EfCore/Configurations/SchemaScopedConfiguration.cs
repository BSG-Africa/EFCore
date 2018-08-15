namespace Bsg.EfCore.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class SchemaScopedConfiguration<TType> : IEntityTypeConfiguration<TType>
        where TType : class
    {
        private readonly string schemaName;

        protected SchemaScopedConfiguration(string schemaName)
        {
            this.schemaName = schemaName;
        }

        public void Configure(EntityTypeBuilder<TType> builder)
        {
            builder.ToTable(typeof(TType).Name, this.schemaName);
            this.OnConfigure(builder);
        }

        protected abstract void OnConfigure(EntityTypeBuilder<TType> builder);
    }
}