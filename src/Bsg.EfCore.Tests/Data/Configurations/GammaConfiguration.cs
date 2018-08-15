namespace Bsg.EfCore.Tests.Data.Configurations
{
    using Domain;
    using EfCore.Configurations;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GammaConfiguration : TestConfiguration<Gamma>
    {
        protected override void OnConfigure(EntityTypeBuilder<Gamma> builder)
        {
            builder.Property(e => e.Category).HasMaxLength(20);
            builder.Property(e => e.Cost).HasPrecision(19, 2);
            builder.Property(e => e.Price).HasPrecision(19, 2);
        }
    }
}
