namespace Bsg.EfCore.Tests.Data.Configurations
{
    using Domain;
    using EfCore.Configurations;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DeltaConfiguration : TestConfiguration<Delta>
    {
        protected override void OnConfigure(EntityTypeBuilder<Delta> builder)
        {
            builder.Property(e => e.Category).HasMaxLength(20);
            builder.Property(e => e.NullDec).HasPrecision(19, 2);
            builder.Property(e => e.Dec).HasPrecision(19, 2);
        }
    }
}
