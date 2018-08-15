namespace Bsg.EfCore.Tests.Data.Configurations
{
    using Domain;
    using EfCore.Configurations;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BetaConfiguration : SchemaScopedConfiguration<Beta>
    {
        public BetaConfiguration()
        : base("CustomSchema")
        {
        }

        protected override void OnConfigure(EntityTypeBuilder<Beta> builder)
        {
            builder.Property(e => e.Code).HasMaxLength(10);
        }
    }
}
