namespace Bsg.EfCore.Tests.Data.Configurations
{
    using Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AlphaConfiguration : TestConfiguration<Alpha>
    {
        protected override void OnConfigure(EntityTypeBuilder<Alpha> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(50);
        }
    }
}
